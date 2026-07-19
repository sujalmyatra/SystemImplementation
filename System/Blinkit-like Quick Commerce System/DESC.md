### Blinkit-like Quick Commerce System

The Blinkit-like Quick Commerce System is designed to manage customers, products, dark stores, cart, orders, payments, delivery partners, discounts, stock availability, and real-time delivery tracking. The system allows customers to order grocery or daily-use items from nearby stores and get fast delivery.

### Main Entities

The system will have the following main entities:

1. **Customer**
   A customer represents a user who places orders.

2. **Product**
   A product represents an item such as milk, bread, fruits, snacks, vegetables, etc.

3. **Store**
   A store represents a nearby dark store or warehouse from where products are delivered.

4. **Stock**
   Stock represents how much quantity of a product is available in a particular store.

5. **Cart**
   A cart represents products selected by the customer before placing an order.

6. **Order**
   An order represents the final purchase made by the customer.

7. **Order Item**
   An order item represents one product inside an order.

8. **Payment**
   Payment represents the payment made for an order.

9. **Delivery Partner**
   A delivery partner represents the person who delivers the order to the customer.

---

### Relationships

1. **Customer to Cart**
   One customer can have one active cart.

2. **Customer to Order**
   One customer can place multiple orders.

3. **Store to Stock**
   One store can have stock of multiple products.

4. **Product to Stock**
   One product can be available in multiple stores with different quantities.

5. **Order to Order Item**
   One order can have multiple order items.

6. **Product to Order Item**
   One product can appear in multiple order items.

7. **Store to Order**
   One store can receive multiple orders.

8. **Delivery Partner to Order**
   One delivery partner can deliver multiple orders over time, but one delivery partner should handle only one active order at a time.

9. **Order to Payment**
   One order can have one payment.

---

### Order Status

The system should support the following order statuses:

1. **Placed**
   Order is created by the customer.

2. **Confirmed**
   Stock and payment are confirmed.

3. **Packed**
   Store staff has packed the items.

4. **OutForDelivery**
   Delivery partner has picked up the order.

5. **Delivered**
   Order is delivered to the customer.

6. **Cancelled**
   Order is cancelled.

7. **Failed**
   Order failed due to payment failure, stock issue, or delivery issue.

---

### Cart Rules

The system should allow customers to add products to cart.

1. Customer can add multiple products to cart.

2. Customer cannot add inactive products.

3. Customer cannot add more quantity than available stock.

4. If same product is already in cart, quantity should be updated instead of adding duplicate item.

5. Cart should show current price, available quantity, and estimated delivery time.

Example:

If Milk is already in cart with quantity 1 and customer adds 2 more, cart quantity should become 3.

---

### Store Selection Rules

The system should select the nearest available store based on customer location or pincode.

1. Customer can order only from serviceable areas.

2. If customer location is not serviceable, order should not be allowed.

3. Order should be assigned to the nearest store that has all required products in stock.

4. If one store does not have all items, system can either reject order or show unavailable items.

Example:

Customer orders Milk, Bread, and Eggs.
Nearest Store A has all 3 items, so order is assigned to Store A.

---

### Stock Rules

The system should manage stock in real time.

1. Product stock should decrease after order confirmation.

2. Product stock should not go negative.

3. If stock quantity becomes 0, product should be shown as Out of Stock.

4. If stock quantity is less than 10, product should be shown as Low Stock.

5. Cancelled orders should restore stock if stock was already deducted.

Example:

Store A has 5 units of Milk.
Customer orders 2 units.
Remaining stock = 3 units.

---

### Discount Rules

The system should support discount and delivery charge rules.

1. Discount can be applied using coupon code or automatic offer.

2. Only one coupon can be applied on one order.

3. Discount should not be greater than order amount.

4. Coupon should have minimum order amount.

5. Coupon should have maximum discount limit.

Example:

Coupon: 10% off up to ₹50
Order Amount = ₹400
Discount = ₹40

Order Amount = ₹800
10% = ₹80, but max discount is ₹50
Final Discount = ₹50

---

### Delivery Charge Rules

The system should calculate delivery charge based on order amount.

1. If order amount is less than or equal to ₹99, delivery charge should be applied.

2. If order amount is greater than ₹99, delivery should be free.

Example:

Order Amount = ₹80
Delivery Charge = ₹25
Final Amount = ₹105

Order Amount = ₹150
Delivery Charge = ₹0
Final Amount = ₹150

Formula:

Final Amount = Order Amount - Discount + Delivery Charge

---

### Payment Rules

The system should handle online payment and cash on delivery.

1. Payment should be linked with an order.

2. Payment amount should match final order amount.

3. Payment status can be:

* Pending
* Success
* Failed
* Refunded

4. If payment fails, order should not be confirmed.

5. If order is cancelled after payment success, refund should be initiated.

---

### Delivery Partner Rules

The system should assign delivery partners to confirmed orders.

1. Only available delivery partners can be assigned.

2. One delivery partner should not handle two active orders at the same time.

3. Delivery partner can be assigned only after order is confirmed or packed.

4. If no delivery partner is available, order should remain in Packed status.

5. After delivery, delivery partner should become available again.

Example:

If Delivery Partner A is delivering Order 1, the system cannot assign Order 2 to the same partner until Order 1 is delivered.

---

### Business Rules

1. Customer cannot order from unserviceable location.

2. Inactive products should not be ordered.

3. Product stock should not become negative.

4. Order cannot be placed with empty cart.

5. Order cannot be confirmed if payment fails.

6. Order status should follow proper sequence.

Example:

Placed → Confirmed → Packed → OutForDelivery → Delivered

7. Order cannot directly move from Placed to Delivered.

8. Delivered order cannot be cancelled.

9. Cancelled paid order should trigger refund.

10. Free delivery should apply only when order amount is greater than ₹99.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Customer
* Product
* Store
* Order
* Payment
* Delivery Partner

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve customer history, order history, payment records, and stock movement history.

---

### Reports

The system should provide the following reports:

#### 1. Store-wise Order Report

Show how many orders each store received in a selected date range.

Example:

* Store A: 120 orders
* Store B: 90 orders

---

#### 2. Real-time Pending Order Report

Show orders that are placed but not yet delivered.

Example:

* Order 1: Confirmed
* Order 2: Packed
* Order 3: OutForDelivery

---

#### 3. Low Stock Product Report

Show products whose stock is below the minimum limit.

Example:

If product quantity is less than 10, show it in low stock report.

* Milk: 5 units
* Bread: 8 units

---

#### 4. Out of Stock Product Report

Show products whose stock quantity is 0.

Example:

* Eggs: 0 units
* Butter: 0 units

---

#### 5. Fast-Moving Product Report

Show products that are ordered frequently in the last month.

Example:

* Milk: 500 units sold
* Bread: 350 units sold

---

#### 6. Slow-Moving Product Report

Show products that are not ordered frequently.

A product should be considered slow-moving if:

* It was not ordered in the last month, or
* It was ordered fewer than 5 times in the last month

---

#### 7. Revenue Report

Show total revenue from delivered orders.

Formula:

Revenue = Sum of final amount of delivered orders

Example:

July 2026 revenue = ₹5,00,000

---

#### 8. Discount Usage Report

Show how much discount was given in a selected month.

Example:

* Total Discount Given: ₹50,000
* Most Used Coupon: SAVE50

---

#### 9. Delivery Partner Performance Report

Show how many orders each delivery partner delivered.

Example:

* Partner A: 80 deliveries
* Partner B: 65 deliveries

---

#### 10. Late Delivery Report

Show orders that were delivered after expected delivery time.

Example:

Expected Delivery Time = 20 minutes
Actual Delivery Time = 35 minutes
Delay = 15 minutes

---

#### 11. Order Status Report

Show order count status-wise.

Example:

* Placed: 10
* Confirmed: 20
* Packed: 15
* OutForDelivery: 8
* Delivered: 150
* Cancelled: 12

---

#### 12. Free Delivery Impact Report

Show how many orders received free delivery and how much delivery charge was collected.

Example:

* Free Delivery Orders: 300
* Paid Delivery Orders: 120
* Delivery Charge Collected: ₹3,000

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show store-wise order count for last month.

2. Show real-time pending orders.

3. Show low stock products store-wise.

4. Show out-of-stock products.

5. Show top 5 fast-moving products.

6. Show slow-moving products for last month.

7. Show total revenue for current month.

8. Show total discount given in current month.

9. Show delivery partner-wise delivered order count.

10. Show late delivery orders.

11. Show order-status-wise count.

12. Show customers who placed more than 5 orders in last month.

13. Show products that were never ordered.

14. Show orders where delivery charge was applied.

15. Show orders where delivery was free.

---

### Overall System Goal

The main goal of this system is to manage quick grocery orders, store-wise stock, customer carts, discounts, payments, delivery partners, and real-time order tracking. The system should prevent invalid orders, maintain real-time stock correctly, apply delivery charges and discounts properly, track delivery status, and generate useful reports for business decision-making.
