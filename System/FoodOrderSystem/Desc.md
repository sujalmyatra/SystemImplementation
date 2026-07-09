## Food Delivery System

The Food Delivery System is designed to manage restaurants, menu items, customers, orders, delivery partners, order status, and delivery tracking. The system allows customers to place food orders from restaurants and assigns delivery partners to deliver the orders.

### Main Entities

The system will have the following main entities:

1. **Restaurant**
   A restaurant represents a food business that sells menu items.

2. **Menu Item**
   A menu item represents food available in a restaurant, such as pizza, burger, dosa, sandwich, etc.

3. **Customer**
   A customer represents a user who places food orders.

4. **Order**
   An order represents food ordered by a customer from a restaurant.

5. **Order Item**
   An order item represents individual menu items inside an order.

6. **Delivery Partner**
   A delivery partner represents the person who delivers the order to the customer.

---

### Relationships

1. **Restaurant to Menu Item**
   One restaurant can have multiple menu items.

2. **Customer to Order**
   One customer can place multiple orders.

3. **Restaurant to Order**
   One restaurant can receive multiple orders.

4. **Order to Order Item**
   One order can have multiple order items.

5. **Menu Item to Order Item**
   One menu item can appear in multiple order items.

6. **Delivery Partner to Order**
   One delivery partner can deliver multiple orders over time, but at a time one delivery partner should handle only one active order.

---

### Order Status

The system should support the following order statuses:

1. **Placed**
   Order is created by the customer.

2. **Accepted**
   Restaurant has accepted the order.

3. **Preparing**
   Restaurant is preparing the food.

4. **ReadyForPickup**
   Food is ready for delivery partner pickup.

5. **OutForDelivery**
   Delivery partner has picked up the order.

6. **Delivered**
   Order has been delivered to the customer.

7. **Cancelled**
   Order has been cancelled.

---

### Order Rules

The system should allow customers to place orders from restaurants.

1. Customer can place an order from one restaurant at a time.

2. One order can contain multiple menu items.

3. Order total should be calculated based on menu item price and quantity.

Formula:

Order Total = Sum of Menu Item Price × Quantity

Example:

Burger = ₹100 × 2 = ₹200
Pizza = ₹250 × 1 = ₹250

Total Order Amount = ₹450

4. Customer cannot order an unavailable menu item.

5. Customer cannot place order from an inactive restaurant.

6. Cancelled orders should not be delivered.

7. Delivered orders cannot be cancelled.

---

### Delivery Partner Rules

The system should assign delivery partners to orders.

1. Only available delivery partners can be assigned to an order.

2. One delivery partner cannot handle two active orders at the same time.

3. Delivery partner can be assigned only after restaurant accepts the order.

4. If no delivery partner is available, the order should remain accepted but delivery assignment should be pending.

5. Once order is delivered, delivery partner should become available again.

Example:

If Delivery Partner A is already delivering Order 1, then Delivery Partner A cannot be assigned to Order 2 until Order 1 is delivered.

---

### Restaurant Rules

The system should manage restaurants and menu items.

1. Restaurant can add, update, or remove menu items.

2. If restaurant is inactive, customers should not be able to place orders from that restaurant.

3. If menu item is unavailable, customers should not be able to order it.

4. Restaurant can accept or reject placed orders.

5. Restaurant should not prepare cancelled orders.

---

### Payment Rules

The system should generate order amount based on selected menu items.

1. Total amount should be calculated automatically.

2. Delivery charge can be added to the final amount.

Formula:

Final Amount = Order Total + Delivery Charge

3. Payment status can be:

* Pending
* Paid
* Failed
* Refunded

4. If payment fails, order should not move forward for preparation.

---

### Business Rules

1. Order should not be accepted if restaurant is inactive.

2. Order should not contain unavailable menu items.

3. Order status should follow proper sequence.

Example:

Placed → Accepted → Preparing → ReadyForPickup → OutForDelivery → Delivered

4. Order cannot directly move from Placed to Delivered.

5. Order cannot be cancelled after it is delivered.

6. Delivery partner should become unavailable when assigned to an active order.

7. Delivery partner should become available after order delivery.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Restaurant
* Menu Item
* Customer
* Order
* Delivery Partner

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve order history, customer history, and restaurant sales records.

---

### Reports

The system should provide the following reports:

#### 1. Restaurant-wise Order Report

Show how many orders each restaurant received in a selected date range.

Example:

* Restaurant A: 100 orders
* Restaurant B: 75 orders

---

#### 2. Restaurant Revenue Report

Show total revenue generated by each restaurant.

Formula:

Restaurant Revenue = Sum of delivered order amounts

Example:

Restaurant A:

* Total delivered orders: 100
* Total revenue: ₹50,000

---

#### 3. Customer Order History Report

Show all orders placed by a selected customer.

Example:

Customer A:

* 1 July 2026: Order from Restaurant X
* 5 July 2026: Order from Restaurant Y

---

#### 4. Most Ordered Menu Item Report

Show menu items that are ordered most frequently.

Example:

* Burger: 200 times
* Pizza: 150 times
* Dosa: 90 times

---

#### 5. Delivery Partner Report

Show how many orders each delivery partner delivered.

Example:

* Delivery Partner A: 80 deliveries
* Delivery Partner B: 65 deliveries

---

#### 6. Order Status Report

Show order count status-wise.

Example:

* Placed: 20
* Accepted: 15
* Preparing: 10
* Delivered: 100
* Cancelled: 8

---

#### 7. Cancelled Order Report

Show cancelled orders in a selected date range.

Example:

* Order 1 cancelled by customer
* Order 2 cancelled by restaurant

---

#### 8. Fast-Moving Menu Item Report

Show menu items that are ordered frequently.

Example:

If Burger was ordered 100 times in the last month, it should be considered fast-moving.

---

#### 9. Slow-Moving Menu Item Report

Show menu items that are not ordered frequently.

A menu item should be considered slow-moving if:

* It was not ordered in the last month, or
* It was ordered fewer than 5 times in the last month

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show restaurant-wise order count for last month.

2. Show restaurant-wise revenue for delivered orders.

3. Show customer order history ordered by latest order first.

4. Show top 5 most ordered menu items.

5. Show delivery partner-wise completed delivery count.

6. Show order-status-wise count for the current month.

7. Show customers who placed more than 5 orders in the last month.

8. Show menu items that were never ordered.

9. Show slow-moving menu items for last month.

10. Show cancelled orders with restaurant and customer details.

11. Show restaurants with no orders in the current month.

12. Show delivery partners who have no deliveries in the current month.

---

### Overall System Goal

The main goal of this system is to manage restaurant food orders, menu items, customers, delivery partners, order status, payments, and reports. The system should prevent invalid orders, avoid assigning one delivery partner to multiple active orders, track order status properly, and generate useful business reports for restaurants and delivery management.
