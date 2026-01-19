using System.Security.Principal;
using ECommerce.Api.Data;
using ECommerce.Api.DTOs;
using ECommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Api.Service;

public class OrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> CreateOrderAsync(CreateOrder request)
    {
        try
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var inv = await _context.Inventories.FirstAsync(x => x.Sku == item.Sku);

                if (inv.ActualQty - inv.ReservedQty < item.Qty)
                {
                    var message = "Stock " + item.Sku + " tidak cukup";
                    throw new Exception(message);
                }

                inv.ReservedQty += item.Qty;

                orderItems.Add(new OrderItem
                {
                    Sku = item.Sku,
                    Qty = item.Qty
                });
            }

            var order = new Order
            {
                UserId = request.UserID,
                Status = OrderStatus.Placed,
                Items = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return order;
        }
        catch (System.Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }

    }

    public async Task<Order> PayOrderAsync(int OrderID, string? paymentExternalID = null)
    {
        try
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            //1. valdasi data order
            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.OrderID == OrderID);

            if (order == null)
                throw new Exception("Order not found");

            if (order.Status != OrderStatus.Placed && order.Status != OrderStatus.Paid)
                throw new Exception("Order tidak bisa dibayar");

            //2. valiadsi idempotency
            var generateExternalID = "PAY-" + OrderID + Guid.NewGuid().ToString().Substring(0, 8);
            paymentExternalID ??= generateExternalID;

            var existingPayment = await _context.Payments
                .FirstOrDefaultAsync(x => x.PaymentExternalID == paymentExternalID);

            if (existingPayment != null)
                return order;

            //3. payment gateway
            var payment = new Payment
            {
                OrderID = OrderID,
                PaymentExternalID = paymentExternalID,
                Amount = order.Items.Sum(x => x.Qty * x.Price),
                PaidAt = DateTimeOffset.Now
            };

            _context.Payments.Add(payment);

            //4.mengurangi actual qty dan reserve qty
            foreach (var item in order.Items)
            {
                var inv = await _context.Inventories.FirstAsync(x => x.Sku == item.Sku);

                if (inv.ReservedQty < item.Qty || inv.ActualQty < item.Qty)
                {
                    var message = "Stock " + item.Sku + " tidak cukup";
                    throw new Exception(message);
                }

                inv.ReservedQty -= item.Qty;
                inv.ActualQty -= item.Qty;
            }

            order.Status = OrderStatus.Paid;
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return order;
        }
        catch (System.Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<Order> CancelOrderAsync(int OrderID)
    {
        try
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            //1. validasi data order
            var order = await _context.Orders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.OrderID == OrderID);

            if (order == null)
                throw new Exception("Order Not Found");

            if (order.Status == OrderStatus.Ship)
                throw new Exception("Order sudah dikirim, tidak bisa di batalkan");

            if (order.Status == OrderStatus.Cancel)
                return order;

            //2. mengembalikan reserved qty
            foreach (var item in order.Items)
            {
                var inv = await _context.Inventories.FirstAsync(x => x.Sku == item.Sku);

                inv.ReservedQty -= item.Qty;
            }

            order.Status = OrderStatus.Cancel;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return order;
        }
        catch (System.Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}