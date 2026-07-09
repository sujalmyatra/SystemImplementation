## Courier / Parcel Tracking System

The Courier / Parcel Tracking System is designed to manage customers, parcels, delivery agents, shipment status, pickup, delivery, and parcel tracking. The system allows customers to send parcels and track the shipment status until the parcel is delivered.

### Main Entities

The system will have the following main entities:

1. **Customer**
   A customer represents a person who sends or receives parcels.

2. **Parcel**
   A parcel represents the package that needs to be picked up and delivered.

3. **Delivery Agent**
   A delivery agent represents the person responsible for picking up and delivering parcels.

4. **Shipment**
   A shipment represents the delivery process of a parcel from sender to receiver.

5. **Shipment Tracking**
   Shipment tracking represents status updates of a shipment over time.

---

### Relationships

1. **Customer to Parcel**
   One customer can send multiple parcels.

2. **Parcel to Shipment**
   One parcel can have one shipment.

3. **Delivery Agent to Shipment**
   One delivery agent can handle multiple shipments over time, but at a time one agent should handle only limited active shipments.

4. **Shipment to Shipment Tracking**
   One shipment can have multiple tracking updates.

---

### Shipment Status

The system should support the following shipment statuses:

1. **Created**
   Parcel is registered in the system.

2. **PickedUp**
   Parcel is picked up from sender.

3. **InTransit**
   Parcel is moving between locations.

4. **OutForDelivery**
   Parcel is with delivery agent for final delivery.

5. **Delivered**
   Parcel is delivered to receiver.

6. **Cancelled**
   Shipment is cancelled before delivery.

7. **Returned**
   Parcel is returned to sender.

---

### Parcel Rules

The system should allow customers to create parcel delivery requests.

1. A customer can send multiple parcels.

2. Parcel should have sender details, receiver details, pickup address, delivery address, weight, and delivery charge.

3. Parcel weight must be greater than 0.

4. Delivery charge should be calculated based on parcel weight and distance.

Formula:

Delivery Charge = Base Charge + Weight Charge + Distance Charge

Example:

Base Charge = ₹50
Weight Charge = ₹20 × 2 kg = ₹40
Distance Charge = ₹5 × 10 km = ₹50

Total Delivery Charge = ₹140

5. Cancelled parcels should not be picked up.

6. Delivered parcels cannot be cancelled.

---

### Shipment Rules

The system should create a shipment for each parcel.

1. Shipment should be created after parcel request is created.

2. Shipment status should follow the correct sequence.

Example:

Created → PickedUp → InTransit → OutForDelivery → Delivered

3. Shipment cannot directly move from Created to Delivered.

4. Cancelled shipment cannot move to InTransit or Delivered.

5. Delivered shipment cannot be updated again.

6. Each shipment status update should be stored in shipment tracking history.

---

### Delivery Agent Rules

The system should assign delivery agents to shipments.

1. Only active delivery agents can be assigned to shipments.

2. One delivery agent can handle multiple shipments, but not more than 5 active shipments at a time.

3. Delivery agent can be assigned only to Created, PickedUp, or InTransit shipments.

4. If no delivery agent is available, shipment should remain unassigned.

5. Once shipment is delivered or returned, it should not count as active shipment for the agent.

Example:

If Delivery Agent A already has 5 active shipments, the system should not assign a new shipment to that agent.

---

### Tracking Rules

The system should maintain shipment tracking history.

1. Every status change should create a tracking record.

2. Tracking record should store shipment status, location, date, and remarks.

3. Customer should be able to view latest shipment status.

4. Customer should also be able to view full tracking history.

Example:

Shipment Tracking:

* 1 July 2026, 10:00 AM: Created at Rajkot
* 1 July 2026, 2:00 PM: PickedUp from sender
* 2 July 2026, 11:00 AM: InTransit at Ahmedabad
* 3 July 2026, 9:00 AM: OutForDelivery
* 3 July 2026, 2:00 PM: Delivered

---

### Business Rules

1. Parcel weight should be valid.

2. Delivery charge should be calculated automatically.

3. Shipment status should follow proper sequence.

4. Delivered shipment cannot be cancelled.

5. Cancelled shipment cannot be delivered.

6. Delivery agent should not exceed active shipment limit.

7. Inactive delivery agent should not receive new shipment.

8. Shipment tracking history should not be deleted permanently.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Customer
* Parcel
* Delivery Agent
* Shipment
* Shipment Tracking

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve shipment history, customer history, and tracking records.

---

### Reports

The system should provide the following reports:

#### 1. Customer-wise Parcel Report

Show how many parcels each customer sent in a selected date range.

Example:

* Customer A: 20 parcels
* Customer B: 12 parcels

---

#### 2. Delivery Agent Performance Report

Show how many shipments each delivery agent delivered.

Example:

* Agent A: 100 deliveries
* Agent B: 75 deliveries

---

#### 3. Shipment Status Report

Show shipment count status-wise.

Example:

* Created: 15
* PickedUp: 20
* InTransit: 40
* Delivered: 120
* Cancelled: 5

---

#### 4. Revenue Report

Show total delivery charges collected from delivered shipments.

Formula:

Revenue = Sum of delivery charges of delivered parcels

Example:

July revenue = ₹1,50,000

---

#### 5. Pending Delivery Report

Show shipments that are not yet delivered.

Example:

* Shipment 1: InTransit
* Shipment 2: OutForDelivery

---

#### 6. Returned Parcel Report

Show parcels that were returned to sender.

Example:

* Parcel A returned due to wrong address
* Parcel B returned because receiver was unavailable

---

#### 7. Late Delivery Report

Show shipments that were delivered after expected delivery date.

Example:

Expected Delivery: 3 July 2026
Actual Delivery: 5 July 2026
Delay: 2 days

---

#### 8. Top Customers Report

Show customers who sent the highest number of parcels.

Example:

* Customer A: 50 parcels
* Customer B: 35 parcels

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show customer-wise parcel count for last month.

2. Show delivery-agent-wise delivered shipment count.

3. Show shipment-status-wise count for current month.

4. Show total revenue from delivered shipments.

5. Show pending deliveries ordered by oldest shipment first.

6. Show returned parcels in a selected date range.

7. Show late deliveries where actual delivery date is greater than expected delivery date.

8. Show top 5 customers by parcel count.

9. Show delivery agents who have more than 5 active shipments.

10. Show shipments that have no tracking update after pickup.

11. Show customers who sent more than 10 parcels in last month.

12. Show delivery agents with no deliveries in current month.

---

### Overall System Goal

The main goal of this system is to manage parcel delivery requests, shipment status, delivery agents, tracking history, and courier reports. The system should prevent invalid status updates, assign delivery agents properly, maintain complete shipment tracking history, and generate useful reports for delivery and revenue analysis.
