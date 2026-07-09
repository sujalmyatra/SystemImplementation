## Loan Management System

The Loan Management System is designed to manage customers, loans, EMI schedules, repayments, and overdue tracking. The system allows customers to take loans, generates EMI records, tracks monthly repayments, and identifies overdue EMIs.

### Main Entities

The system will have the following main entities:

1. **Customer**
   A customer represents a person who applies for or takes a loan.

2. **Loan**
   A loan represents the amount borrowed by a customer from the bank or finance company.

3. **EMI**
   EMI represents the monthly installment that the customer needs to pay for a loan.

4. **Repayment**
   A repayment represents the amount paid by the customer against an EMI.

5. **Overdue Record**
   An overdue record represents EMI payments that were not paid on or before the due date.

---

### Relationships

1. **Customer to Loan**
   One customer can have multiple loans.

2. **Loan to EMI**
   One loan can have multiple EMI records.

3. **EMI to Repayment**
   One EMI can have one or more repayments.

4. **Loan to Repayment**
   One loan can have multiple repayments over time.

5. **Customer to Repayment**
   One customer can make multiple repayments.

---

### Loan Status

The system should support the following loan statuses:

1. **Pending**
   Loan application is created but not approved.

2. **Approved**
   Loan is approved but amount is not disbursed.

3. **Active**
   Loan amount is disbursed and EMI schedule is generated.

4. **Closed**
   Loan is fully repaid.

5. **Rejected**
   Loan application is rejected.

6. **Defaulted**
   Loan has too many overdue EMIs.

---

### EMI Status

The system should support the following EMI statuses:

1. **Pending**
   EMI is not paid yet.

2. **PartiallyPaid**
   EMI is partially paid.

3. **Paid**
   EMI is fully paid.

4. **Overdue**
   EMI due date has passed and full payment is not completed.

---

### Loan Rules

The system should allow customers to apply for loans.

1. Customer can apply for multiple loans.

2. Loan amount must be greater than 0.

3. Loan duration must be greater than 0 months.

4. Interest rate must be greater than 0.

5. EMI schedule should be generated only after loan is approved and activated.

6. Rejected loans should not generate EMI records.

7. Closed loans should not accept further repayments.

Example:

If customer takes a loan of ₹1,20,000 for 12 months, the system should generate 12 EMI records.

---

### EMI Rules

The system should generate monthly EMI records for active loans.

1. EMI should have due date, EMI amount, paid amount, and status.

2. EMI amount should be calculated based on loan amount, interest rate, and duration.

Simple Formula:

Total Payable Amount = Loan Amount + Interest Amount

EMI Amount = Total Payable Amount / Number of Months

Example:

Loan Amount = ₹1,00,000
Interest Amount = ₹12,000
Duration = 12 months

Total Payable = ₹1,12,000
EMI = ₹1,12,000 / 12 = ₹9,333.33

3. EMI status should update automatically based on paid amount.

Example:

EMI Amount = ₹10,000
Paid Amount = ₹0 → Pending
Paid Amount = ₹5,000 → PartiallyPaid
Paid Amount = ₹10,000 → Paid

4. If due date is passed and EMI is not fully paid, EMI should be marked as Overdue.

---

### Repayment Rules

The system should allow customers to make repayments against EMIs.

1. Repayment amount must be greater than 0.

2. Repayment cannot be greater than pending EMI amount.

3. Repayment should update EMI paid amount.

4. If EMI is fully paid, EMI status should become Paid.

5. Partial payment should mark EMI as PartiallyPaid.

6. Repayment should be allowed only for active loans.

Example:

If EMI amount is ₹10,000 and customer pays ₹4,000, EMI status should become PartiallyPaid and pending amount should be ₹6,000.

---

### Overdue Rules

The system should track overdue EMIs.

1. If EMI due date is passed and EMI is not fully paid, it should be marked as Overdue.

2. Overdue EMI should create an overdue record.

3. If a customer has more than 3 overdue EMIs for the same loan, loan status should be marked as Defaulted.

4. If overdue EMI is later paid fully, EMI status should become Paid.

5. Overdue records should be preserved for history and reporting.

Example:

If Customer A has 4 unpaid overdue EMIs for Loan X, the loan should be marked as Defaulted.

---

### Business Rules

1. Customer cannot take a loan with invalid amount or invalid duration.

2. EMI schedule should not be generated before loan activation.

3. Repayment should not be allowed for closed or rejected loans.

4. EMI paid amount should not exceed EMI amount.

5. Loan should be closed only when all EMIs are paid.

6. Loan should become defaulted if overdue limit is crossed.

7. Overdue history should not be deleted permanently.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Customer
* Loan
* EMI
* Repayment
* Overdue Record

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve loan history, repayment history, and overdue records.

---

### Reports

The system should provide the following reports:

#### 1. Customer-wise Loan Report

Show how many loans each customer has taken.

Example:

* Customer A: 2 loans
* Customer B: 1 loan

---

#### 2. Loan Status Report

Show loan count status-wise.

Example:

* Pending: 10
* Active: 50
* Closed: 30
* Defaulted: 5

---

#### 3. EMI Collection Report

Show total EMI amount collected in a selected month.

Example:

July 2026 EMI Collection = ₹5,00,000

---

#### 4. Pending EMI Report

Show EMIs that are not fully paid.

Example:

* Customer A: EMI ₹10,000, Paid ₹4,000, Pending ₹6,000
* Customer B: EMI ₹8,000, Paid ₹0, Pending ₹8,000

---

#### 5. Overdue EMI Report

Show EMIs whose due date has passed and payment is not completed.

Example:

* Customer A: EMI due on 5 July 2026
* Customer B: EMI due on 10 July 2026

---

#### 6. Defaulted Loan Report

Show loans that have more than 3 overdue EMIs.

Example:

* Loan A: 4 overdue EMIs
* Loan B: 5 overdue EMIs

---

#### 7. Customer Repayment History Report

Show repayment history of a selected customer.

Example:

Customer A:

* 1 July 2026: Paid ₹10,000
* 1 August 2026: Paid ₹10,000

---

#### 8. Monthly Loan Disbursement Report

Show total loan amount disbursed in a selected month.

Example:

July 2026 loan disbursement = ₹20,00,000

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show customer-wise loan count.

2. Show loan-status-wise count.

3. Show EMI collection amount for current month.

4. Show pending EMIs ordered by due date.

5. Show overdue EMIs for last month.

6. Show customers who have more than 3 overdue EMIs.

7. Show defaulted loans.

8. Show customer repayment history ordered by latest payment first.

9. Show loans where all EMIs are paid.

10. Show active loans with pending EMI count.

11. Show top 5 customers by total loan amount.

12. Show monthly loan disbursement amount.

---

### Overall System Goal

The main goal of this system is to manage customer loans, EMI schedules, repayments, overdue tracking, and loan reports. The system should generate EMIs correctly, track repayments, prevent invalid payments, identify overdue EMIs, mark defaulted loans, and preserve loan and repayment history for reporting.
