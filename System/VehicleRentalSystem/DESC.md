## Vehicle Rental System

The Vehicle Rental System is designed to manage vehicles, customers, bookings, rental charges, vehicle availability, and late return penalties. The system allows customers to book vehicles for a selected date range, calculates rental charges, tracks returns, and applies late fees when vehicles are returned after the expected return date.

### Main Entities

The system will have the following main entities:

1. **Vehicle**
   A vehicle represents a car, bike, scooter, or any other vehicle available for rent.

2. **Customer**
   A customer represents a person who rents a vehicle.

3. **Booking**
   A booking represents a rental request made by a customer for a vehicle.

4. **Rental Payment**
   A rental payment represents the amount paid by the customer for the booking.

5. **Vehicle Return**
   A vehicle return represents the return details of the rented vehicle.

---

### Relationships

1. **Customer to Booking**
   One customer can have multiple bookings.

2. **Vehicle to Booking**
   One vehicle can have multiple bookings over time, but only one active booking at a time.

3. **Booking to Rental Payment**
   One booking can have one payment.

4. **Booking to Vehicle Return**
   One completed rental booking can have one vehicle return record.

---

### Vehicle Status

The system should support the following vehicle statuses:

1. **Available**
   Vehicle is available for booking.

2. **Booked**
   Vehicle is booked by a customer.

3. **Rented**
   Vehicle is currently with the customer.

4. **UnderMaintenance**
   Vehicle is under repair or service.

5. **Damaged**
   Vehicle is damaged and cannot be rented.

6. **Inactive**
   Vehicle is not available for rental.

---

### Booking Status

The system should support the following booking statuses:

1. **Pending**
   Booking is created but not confirmed.

2. **Confirmed**
   Booking is approved and payment is completed.

3. **Cancelled**
   Booking is cancelled before rental starts.

4. **Active**
   Vehicle is handed over to the customer.

5. **Completed**
   Vehicle is returned successfully.

---

### Vehicle Booking Rules

The system should allow customers to book available vehicles.

1. Customer can book a vehicle by selecting start date and end date.

2. Booking start date should not be greater than booking end date.

3. Vehicle should be available during the selected date range.

4. Same vehicle should not have overlapping active bookings.

Example:

If Car A is booked from 10 July 2026 to 15 July 2026, another customer cannot book Car A from 12 July 2026 to 14 July 2026.

5. Vehicle under maintenance, damaged, or inactive should not be booked.

6. Cancelled bookings should not block vehicle availability.

---

### Rental Charge Rules

The system should calculate rental charges automatically.

1. Each vehicle should have a daily rental rate.

2. Rental days should be calculated from booking start date to booking end date.

3. Total rental charge should be calculated based on daily rate and number of rental days.

Formula:

Total Rental Charge = Daily Rental Rate × Number of Rental Days

Example:

Daily Rate = ₹1000
Rental Days = 3

Total Rental Charge = ₹3000

4. Security deposit can be added if required.

Formula:

Final Amount = Rental Charge + Security Deposit

---

### Payment Rules

The system should handle payment for rental bookings.

1. Payment should be created for a booking.

2. Payment amount should match the final rental amount.

3. Payment status can be:

* Pending
* Paid
* Failed
* Refunded

4. Booking should become confirmed only after successful payment.

5. Failed payment should not confirm booking.

---

### Vehicle Return Rules

The system should track vehicle return details.

1. Vehicle can be returned only for active bookings.

2. Actual return date should be stored.

3. If vehicle is returned on or before expected return date, no late fee should be charged.

4. If vehicle is returned after expected return date, late fee should be calculated.

Formula:

Late Fee = Late Days × Per Day Late Charge

Example:

Expected Return Date = 10 July 2026
Actual Return Date = 12 July 2026
Late Days = 2
Per Day Late Charge = ₹500

Late Fee = ₹1000

5. After return, booking status should become Completed.

6. Vehicle status should become Available, UnderMaintenance, or Damaged based on vehicle condition.

---

### Late Return Rules

The system should identify late returns.

1. If actual return date is greater than booking end date, booking should be considered late.

2. Late fee should be added to final payable amount.

3. If customer has more than 2 late returns, system should show a warning before allowing a new booking.

Example:

Customer A has 3 late returns.
When creating a new booking, system should show:

“Customer has multiple late return records.”

---

### Business Rules

1. Vehicle cannot be booked if it is not available.

2. Vehicle cannot have overlapping active bookings.

3. Booking cannot be confirmed without successful payment.

4. Cancelled booking cannot be started.

5. Completed booking cannot be cancelled.

6. Vehicle return should update vehicle status.

7. Late fee should be calculated automatically.

8. Rental and return history should not be permanently deleted.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Vehicle
* Customer
* Booking
* Rental Payment
* Vehicle Return

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve booking history, payment history, and vehicle return records.

---

### Reports

The system should provide the following reports:

#### 1. Vehicle-wise Booking Report

Show how many times each vehicle was booked in a selected date range.

Example:

* Car A: 20 bookings
* Bike B: 15 bookings

---

#### 2. Customer Booking History Report

Show all bookings made by a selected customer.

Example:

Customer A:

* 1 July 2026: Car A
* 10 July 2026: Bike B

---

#### 3. Revenue Report

Show total rental revenue in a selected month.

Formula:

Revenue = Sum of paid rental booking amounts

Example:

July 2026 rental revenue = ₹2,50,000

---

#### 4. Vehicle Availability Report

Show vehicles that are currently available for booking.

Example:

* Car A
* Bike B
* Scooter C

---

#### 5. Late Return Report

Show bookings where vehicles were returned late.

Example:

* Customer A returned Car X 2 days late
* Customer B returned Bike Y 1 day late

---

#### 6. Vehicle Status Report

Show vehicle count status-wise.

Example:

* Available: 30
* Booked: 10
* Rented: 15
* UnderMaintenance: 5
* Damaged: 2

---

#### 7. Most Rented Vehicle Report

Show vehicles rented most frequently.

Example:

* Car A: 50 bookings
* Bike B: 40 bookings

---

#### 8. Customers With Multiple Late Returns

Show customers who returned vehicles late more than 2 times.

Example:

Customer A has 3 late returns, so show this customer in the report.

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show vehicle-wise booking count for last month.

2. Show customer booking history ordered by latest booking first.

3. Show total rental revenue for current month.

4. Show available vehicles for a selected date range.

5. Show late return bookings.

6. Show customers who have more than 2 late returns.

7. Show vehicle-status-wise count.

8. Show top 5 most rented vehicles.

9. Show bookings with failed or pending payments.

10. Show customers who booked more than 3 vehicles in last month.

11. Show vehicles not booked in the last 6 months.

12. Show total late fee collected in current month.

---

### Overall System Goal

The main goal of this system is to manage vehicle rentals, customer bookings, rental payments, vehicle returns, and late return tracking. The system should prevent overlapping bookings, calculate rental charges correctly, handle payment status, track late returns, update vehicle availability, and generate useful reports for rental business management.
