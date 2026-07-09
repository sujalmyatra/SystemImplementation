## Insurance Policy System

The Insurance Policy System is designed to manage customers, insurance policies, premium calculation, claims, claim approval, and policy status. The system allows customers to buy insurance policies, pay premiums, and raise claims when required.

### Main Entities

The system will have the following main entities:

1. **Customer**
   A customer represents a person who buys an insurance policy.

2. **Policy**
   A policy represents the insurance plan purchased by the customer.

3. **Premium**
   A premium represents the amount the customer needs to pay for the insurance policy.

4. **Claim**
   A claim represents a request made by the customer to receive insurance benefit or compensation.

5. **Claim Document**
   A claim document represents supporting documents submitted for claim verification.

---

### Relationships

1. **Customer to Policy**
   One customer can have multiple policies.

2. **Policy to Premium**
   One policy can have multiple premium payments.

3. **Policy to Claim**
   One policy can have multiple claims.

4. **Claim to Claim Document**
   One claim can have multiple claim documents.

---

### Policy Types

The system can support different policy types:

1. **Health Insurance**
2. **Vehicle Insurance**
3. **Life Insurance**
4. **Travel Insurance**

Each policy type may have different premium calculation rules.

---

### Policy Status

The system should support the following policy statuses:

1. **Pending**
   Policy is created but not approved.

2. **Active**
   Policy is approved and premium payment is valid.

3. **Expired**
   Policy validity period is over.

4. **Cancelled**
   Policy is cancelled by customer or company.

5. **Lapsed**
   Policy became inactive because premium was not paid on time.

---

### Claim Status

The system should support the following claim statuses:

1. **Submitted**
   Claim is created by the customer.

2. **UnderReview**
   Claim is being verified.

3. **Approved**
   Claim is approved.

4. **Rejected**
   Claim is rejected.

5. **Paid**
   Claim amount is paid to the customer.

---

### Policy Rules

The system should allow customers to buy insurance policies.

1. Customer can have multiple policies.

2. Policy start date should be less than policy end date.

3. Policy coverage amount must be greater than 0.

4. Premium amount should be calculated based on policy type, coverage amount, customer age, and risk factor.

5. Expired or cancelled policies should not accept new claims.

6. Lapsed policies should not accept claims until renewed.

Example:

If Customer A buys a Health Insurance policy with ₹5,00,000 coverage, the system should calculate the premium based on age, coverage amount, and policy duration.

---

### Premium Calculation Rules

The system should calculate premium amount automatically.

Simple Formula:

Premium Amount = Base Premium + Risk Charges + Coverage Charges

Example:

Base Premium = ₹5000
Risk Charges = ₹2000
Coverage Charges = ₹3000

Total Premium = ₹10,000

Premium calculation can vary by policy type.

Example:

* Health Insurance may depend on age and medical risk.
* Vehicle Insurance may depend on vehicle type and vehicle age.
* Life Insurance may depend on age and coverage amount.

---

### Premium Payment Rules

The system should track premium payments.

1. Premium amount must be greater than 0.

2. Premium payment should be linked with a policy.

3. Premium status can be:

* Pending
* Paid
* Failed
* Overdue

4. If premium is not paid before due date, policy can become Lapsed.

5. If premium is paid successfully, policy should remain Active.

6. Failed premium payment should not activate policy.

Example:

If premium due date is 10 July 2026 and customer does not pay by that date, policy status can become Lapsed.

---

### Claim Rules

The system should allow customers to raise claims against active policies.

1. Claim can be created only for active policies.

2. Claim amount must be greater than 0.

3. Claim amount cannot be greater than policy coverage amount.

4. Claim should have required documents.

5. Claim cannot be approved if required documents are missing.

6. Rejected claims cannot be paid.

7. Approved claims can be marked as Paid.

8. Total approved claim amount should not exceed policy coverage amount.

Example:

Policy Coverage = ₹5,00,000
Already Approved Claims = ₹3,00,000
New Claim Amount = ₹2,50,000

This claim should not be allowed because total claim amount becomes ₹5,50,000.

---

### Business Rules

1. Customer cannot raise claim for expired, cancelled, or lapsed policy.

2. Premium should be calculated automatically.

3. Policy should become active only after approval and successful premium payment.

4. Claim amount should not exceed remaining coverage amount.

5. Claim cannot move directly from Submitted to Paid.

6. Claim status should follow proper sequence.

Example:

Submitted → UnderReview → Approved → Paid

7. Rejected claim should not be paid.

8. Policy history and claim history should not be permanently deleted.

---

### Soft Delete

Important records should not be permanently deleted from the database.

Soft delete should be used for:

* Customer
* Policy
* Premium
* Claim
* Claim Document

When a record is deleted, it should be marked as:

`IsDeleted = true`

This helps preserve policy history, premium history, and claim records.

---

### Reports

The system should provide the following reports:

#### 1. Customer-wise Policy Report

Show how many policies each customer has purchased.

Example:

* Customer A: 3 policies
* Customer B: 1 policy

---

#### 2. Policy Status Report

Show policy count status-wise.

Example:

* Active: 100
* Expired: 30
* Cancelled: 10
* Lapsed: 15

---

#### 3. Premium Collection Report

Show total premium collected in a selected month.

Example:

July 2026 premium collection = ₹10,00,000

---

#### 4. Pending Premium Report

Show policies whose premium payment is pending or overdue.

Example:

* Policy A: Premium Pending
* Policy B: Premium Overdue

---

#### 5. Claim Status Report

Show claim count status-wise.

Example:

* Submitted: 20
* UnderReview: 15
* Approved: 10
* Rejected: 5
* Paid: 8

---

#### 6. Approved Claim Amount Report

Show total approved claim amount in a selected date range.

Example:

July 2026 approved claims = ₹7,50,000

---

#### 7. Customer Claim History Report

Show claim history of a selected customer.

Example:

Customer A:

* 1 July 2026: Claim ₹50,000 - Approved
* 10 July 2026: Claim ₹20,000 - UnderReview

---

#### 8. High Claim Customers Report

Show customers whose total claim amount is high.

Example:

Customers who claimed more than ₹1,00,000 in the current year.

---

#### 9. Expiring Policies Report

Show policies that are going to expire soon.

Example:

Policies expiring in the next 30 days.

---

### LINQ Query Scenarios

The system should support LINQ queries for:

1. Show customer-wise policy count.

2. Show policy-status-wise count.

3. Show total premium collected for current month.

4. Show pending or overdue premium policies.

5. Show claim-status-wise count.

6. Show customer-wise total claim amount.

7. Show top 5 customers by total premium paid.

8. Show policies expiring in the next 30 days.

9. Show claims where claim amount is greater than ₹1,00,000.

10. Show rejected claims in last month.

11. Show policies with no premium payment.

12. Show customers who have more than 2 active policies.

---

### Overall System Goal

The main goal of this system is to manage insurance customers, policies, premiums, claims, and reports. The system should calculate premiums correctly, prevent invalid claims, track premium payments, maintain policy status, and preserve policy and claim history for business and audit purposes.
