## E-Commerce Order Management System

The E-Commerce Order Management System is designed to manage products, customers, carts, orders, payments, and returns. The system allows customers to add products to cart, place orders, make payments, and request returns for delivered products.

### Main Entities

The system will have the following main entities:

1. **Product**
   A product represents an item available for purchase.

2. **Customer**
   A customer represents a user who buys products from the system.

3. **Cart**
   A cart represents temporary selected products before placing an order.

4. **Cart Item**
   A cart item represents one product inside the customer’s cart with quantity.

5. **Order**
   An order represents the final purchase placed by a customer.

6. **Order Item**
   An order item represents one product inside an order.

7. **Payment**
   A payment represents the payment made for an order.

8. **Return Request**
   A return request represents a customer request to return a delivered product.

---

### Relationships

1. **Customer to Cart**
   One customer can have one active cart.

2. **Cart to Cart Item**
   One cart can have multiple cart items.

3. **Product to Cart Item**
   One product can appear in multiple cart items.

4. **Customer to Order**
   One customer can place multiple orders.

5. **Order to Order Item**
   One order can have multiple order items.

6. **Product to Order Item**
   One product can appear in multiple order items.

7. **Order to Payment**
   One order can have one payment.

8. **Order to Return Request**
   One order can have multiple return requests.

---

### Order Status

The system should support the following order statuses:

1. **Pending**
   Order is created but payment is not completed.

2. **Confirmed**
   Payment is successful and order is confirmed.

3. **Shipped**
   Order has been shipped.

4. **Delivered**
   Order has been delivered to the customer.

5. **Cancelled**
   Order has been cancelled.

6. **Returned**
   Order item has been returned.

---

### Cart Rules

The system should allow customers to add products to cart before placing an order.

1. Customer can add multiple products to cart.

2. Customer cannot add an inactive product to cart.

3. Customer cannot add more quantity than available stock.

4. If the same product is already in cart, quantity should be updated instead of adding duplicate cart item.

Example:

If Product A is already in cart with quantity 2 and customer adds 1 more, cart quantity should become 3.

---

### Order Rules

The system should allow customers to place orders from cart.

1. Customer can place order only if cart has at least one item.

2. Order total should be calculated based on product price and quantity.

Formula:

Order Total = Sum of Product Price × Quantity

Example:

Product A = ₹500 × 2 = ₹1000
Product B = ₹300 × 1 = ₹300

Total Order Amount = ₹1300

3. Product stock should decrease after order is confirmed.

4. Order cannot be confirmed if payment fails.

5. Cancelled orders cannot be shipped.

6. Delivered orders cannot be cancelled.

7. Order status should follow proper sequence.

Example:

Pending → Confirmed → Shipped → Delivered

---

### Payment Rules

The system should handle payment for orders.

1. Payment should be created for an order.

2. Payment amount should match the order total amount.

3. Payment status can be:

* Pending
* Success
* Failed
* Refunded

4. If payment is successful, order status should become Confirmed.

5. If payment fails, order should remain Pending or become Cancelled.

6. Payment should not be greater or less than order total amount.

---

### Return Rules

The system should allow customers to return delivered products.

1. Return request can be created only for delivered orders.

2. Cancelled or pending orders cannot be returned.

3. Return request should be created within 7 days of delivery.

4. One order item should not have multiple active return requests.

5. After return is approved, product stock should increase again.

6. Payment can be refunded after return approval.

Example:

If customer received Product A on 1 July 2026, return can be requested until 8 July 2026.

---

### Business Rules

1. Product stock should not go negative.

2. Inactive products cannot be ordered.

3. Order cannot be placed with empty cart.

4. Payment success is required before confirming order.

5. Order cannot directly move from Pending to Delivered.

6. Delivered order cannot be cancelled.

7. Return is allowed only after delivery.

8. Return should not be allowed after return period is over.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Product
* Customer
* Cart
* Order
* Payment
* Return Request

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve customer history, order history, payment history, and return records.

---

### Reports

The system should provide the following reports:

#### 1. Product Sales Report

Show how many quantities of each product were sold in a selected date range.

Example:

* Product A: 100 units sold
* Product B: 75 units sold

---

#### 2. Customer Order History Report

Show all orders placed by a selected customer.

Example:

Customer A:

* 1 July 2026: Order ₹1500
* 5 July 2026: Order ₹800

---

#### 3. Revenue Report

Show total revenue from successful orders.

Formula:

Revenue = Sum of successful paid order amounts

Example:

July 2026 revenue = ₹2,50,000

---

#### 4. Top Selling Product Report

Show products sold the most.

Example:

* Product A: 500 units
* Product B: 300 units
* Product C: 250 units

---

#### 5. Pending Payment Report

Show orders where payment is pending or failed.

Example:

* Order 1: Payment Pending
* Order 2: Payment Failed

---

#### 6. Return Report

Show returned products in a selected date range.

Example:

* Product A: 10 returns
* Product B: 5 returns

---

#### 7. Customer-wise Purchase Report

Show total purchase amount grouped by customer.

Example:

* Customer A: ₹20,000
* Customer B: ₹12,000

---

#### 8. Low Stock Product Report

Show products whose stock quantity is below a fixed limit.

Example:

If stock quantity is less than 10, show product as Low Stock.

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show customer-wise order count for last month.

2. Show product-wise sold quantity.

3. Show top 5 selling products.

4. Show total revenue for current month.

5. Show orders with pending or failed payment.

6. Show customers who spent more than ₹10,000 in a selected month.

7. Show products that were never ordered.

8. Show order-status-wise count.

9. Show return requests for delivered orders.

10. Show customer order history ordered by latest order first.

11. Show low stock products.

12. Show products with more than 5 returns in last month.

---

### Overall System Goal

The main goal of this system is to manage customer carts, product orders, payments, returns, and reports. The system should prevent invalid orders, maintain stock correctly, handle payment status, allow valid returns, and generate useful business reports for sales and customer analysis.
