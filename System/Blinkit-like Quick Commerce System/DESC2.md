## Blinkit-like Quick Commerce System

The Blinkit-like Quick Commerce System is designed to manage customers, products, stores, stock, carts, orders, payments, delivery partners, discounts, and fast delivery tracking.

---

### Main Entities

1. **Customer**
   Represents a user who places orders.

2. **Product**
   Represents grocery or daily-use items like milk, bread, fruits, snacks, etc.

3. **Store**
   Represents a nearby dark store or warehouse.

4. **Stock**
   Represents product quantity available in a store.

5. **Cart**
   Represents products selected by the customer before order placement.

6. **Order**
   Represents the final purchase made by the customer.

7. **Order Item**
   Represents one product inside an order.

8. **Payment**
   Represents payment made for an order.

9. **Delivery Partner**
   Represents the person who delivers the order.

---

### Relationships

1. One customer can have one active cart.

2. One customer can place multiple orders.

3. One store can have stock of multiple products.

4. One product can be available in multiple stores.

5. One order can have multiple order items.

6. One product can appear in multiple order items.

7. One store can receive multiple orders.

8. One delivery partner can deliver multiple orders over time, but only one active order at a time.

9. One order can have one payment.

---

### Order Status

The system should support:

* Placed
* Confirmed
* Packed
* OutForDelivery
* Delivered
* Cancelled
* Failed

---

### Cart and Stock Rules

1. Customer can add multiple products to cart.

2. Inactive products cannot be added to cart.

3. Customer cannot add more quantity than available stock.

4. If the same product already exists in cart, quantity should be updated.

5. Stock should decrease after order confirmation.

6. Stock should not become negative.

7. If stock is 0, product should be shown as Out of Stock.

8. If stock is less than 10, product should be shown as Low Stock.

---

### Store Selection Rules

1. Customer can order only from serviceable areas.

2. Order should be assigned to the nearest store that has required products in stock.

3. If required products are not available, order should not be confirmed.

---

### Discount Rules

1. Only one coupon can be applied on one order.

2. Coupon should have minimum order amount.

3. Coupon should have maximum discount limit.

4. Discount should not be greater than order amount.

Example:

Coupon: 10% off up to ₹50
Order Amount: ₹800
10% Discount: ₹80
Final Discount: ₹50

---

### Delivery Charge Rules

1. If order amount is less than or equal to ₹99, delivery charge should apply.

2. If order amount is greater than ₹99, delivery should be free.

Example:

Order Amount = ₹80
Delivery Charge = ₹25
Final Amount = ₹105

Order Amount = ₹150
Delivery Charge = ₹0
Final Amount = ₹150

Formula:

`Final Amount = Order Amount - Discount + Delivery Charge`

---

### Payment Rules

1. Payment should be linked with one order.

2. Payment amount should match final order amount.

3. Payment status can be:

* Pending
* Success
* Failed
* Refunded

4. If payment fails, order should not be confirmed.

5. If paid order is cancelled, refund should be initiated.

---

### Delivery Partner Rules

1. Only available delivery partners can be assigned.

2. One delivery partner cannot handle two active orders at the same time.

3. Delivery partner can be assigned only after order is confirmed or packed.

4. If no delivery partner is available, order should remain in Packed status.

5. After delivery, delivery partner should become available again.

---

### Business Rules

1. Customer cannot order from unserviceable location.

2. Order cannot be placed with empty cart.

3. Inactive products cannot be ordered.

4. Product stock should not become negative.

5. Order cannot be confirmed if payment fails.

6. Order status should follow proper sequence:

`Placed → Confirmed → Packed → OutForDelivery → Delivered`

7. Delivered order cannot be cancelled.

8. Cancelled paid order should trigger refund.

9. Free delivery should apply only when order amount is greater than ₹99.

---

### Soft Delete

Soft delete should be used for:

* Customer
* Product
* Store
* Order
* Delivery Partner

When deleted:

`IsDeleted = true`

This helps preserve customer history, order history, store records, and delivery records.

---

### Reports

#### 1. Store-wise Order Report

Show order count for each store in a selected date range.

#### 2. Pending Order Report

Show orders that are not delivered yet.

Example:

* Confirmed
* Packed
* OutForDelivery

#### 3. Low Stock Report

Show products whose stock is less than 10.

#### 4. Out of Stock Report

Show products whose stock is 0.

#### 5. Fast-Moving Product Report

Show products ordered most frequently in last month.

#### 6. Slow-Moving Product Report

Show products that were not ordered or ordered fewer than 5 times in last month.

#### 7. Revenue Report

Show total revenue from delivered orders.

Formula:

`Revenue = Sum of final amount of delivered orders`

#### 8. Delivery Partner Report

Show how many orders each delivery partner delivered.

#### 9. Order Status Report

Show order count status-wise.

#### 10. Free Delivery Report

Show free delivery orders and paid delivery orders.

---

### LINQ Query Scenarios

1. Show store-wise order count for last month.

2. Show pending orders.

3. Show low-stock products store-wise.

4. Show out-of-stock products.

5. Show top 5 fast-moving products.

6. Show slow-moving products.

7. Show total revenue for current month.

8. Show delivery-partner-wise delivered order count.

9. Show order-status-wise count.

10. Show customers who placed more than 5 orders last month.

11. Show products that were never ordered.

12. Show orders where delivery charge was applied.

13. Show orders where delivery was free.

---

### Overall System Goal

The main goal of this system is to manage quick grocery orders, store-wise stock, carts, payments, delivery partners, discounts, delivery charges, and order tracking. The system should prevent invalid orders, maintain stock correctly, apply discounts and delivery charges properly, and generate useful business reports.
