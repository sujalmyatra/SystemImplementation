# User Story: Preferred Payment Method During Checkout

## User Story

**As a** registered customer  
**I want to** select and use a preferred payment method during checkout  
**So that** I can securely complete my order and receive confirmation of payment  

---

## Acceptance Criteria

### 1. Payment Method Selection

- The system must display all available payment methods during checkout.
- The customer must be able to select only one payment method for an order.

### 2. Payment Execution

- The system must process the payment using the selected payment method.
- The system must ensure that payment is completed successfully before confirming the order.

### 3. Transaction Recording

Each payment attempt must be recorded with the following details:

- A unique transaction identifier
- Associated order ID
- Selected payment method
- Transaction timestamp
- Payment status:
  - `Success`
  - `Failed`

### 4. Successful Payment Response

On successful payment, the system must return:

- Order ID
- Payment method used
- Transaction reference number
- Payment timestamp

### 5. Payment Failure Handling

When payment fails, the system must return a structured error response containing:

- Error code
- Error message
- Suggested next action:
  - `Retry`
  - `ChooseAnotherMethod`

### 6. Duplicate Payment Prevention

- The system must prevent multiple successful payments for the same order.
- If an order already has a successful payment, any additional payment attempt must be blocked.
- The system must return an appropriate duplicate-payment error response.

### 7. Extensibility

- The system must support adding new payment methods in the future.
- Adding a new payment method must not require modifying existing payment-processing functionality.
- Existing payment methods must continue to work without being affected.

### 8. Audit and Compliance

- All payment activities must be logged for audit purposes.
- Audit logs should include payment attempts, success, failure, retries, and duplicate-payment attempts.
- Sensitive payment information must not be exposed in API responses or logs.
- Full card numbers, CVV values, PINs, passwords, and authentication tokens must never be stored in plain text.

---

## Business Rules

1. Only one payment method can be selected for an order at a time.
2. Payment must succeed before the order is confirmed.
3. Failed payments must not confirm the order.
4. An order must have no more than one successful payment.
5. A customer may retry a failed payment using the same or a different payment method.
6. Refunds must reference the original successful transaction.
7. Sensitive payment information must not be included in payment responses.
8. Every payment attempt must be recorded for audit and troubleshooting purposes.

---

## Example Scenarios

### Scenario 1: Payment Succeeds

**Given** the customer has an order ready for checkout  
**And** the customer selects an available payment method  
**When** the payment is processed successfully  
**Then** the payment transaction is recorded as `Success`  
**And** the order is confirmed  
**And** the system returns the order ID, payment method, transaction reference, and timestamp  

### Scenario 2: Payment Fails

**Given** the customer has selected a payment method  
**When** the payment provider rejects or fails the payment  
**Then** the payment transaction is recorded as `Failed`  
**And** the order is not confirmed  
**And** the system returns an error code, error message, and suggested next action  

### Scenario 3: Retry With a Different Payment Method

**Given** the customer's previous payment attempt failed  
**When** the customer selects another payment method  
**And** the new payment attempt succeeds  
**Then** the new transaction is recorded as `Success`  
**And** the order is confirmed  
**And** the failed transaction remains available in the payment history  

### Scenario 4: Duplicate Payment Attempt

**Given** an order already has a successful payment  
**When** the customer or system attempts another payment for the same order  
**Then** the payment attempt is blocked  
**And** no additional successful transaction is created  
**And** the system returns a duplicate-payment error response  

---

## Suggested Payment Statuses

```text
Pending
Processing
Success
Failed
Refunded
PartiallyRefunded
```

---

## Suggested Order Statuses

```text
PendingPayment
Confirmed
Cancelled
Completed
```

---

## Suggested Error Response

```json
{
  "errorCode": "PAYMENT_FAILED",
  "errorMessage": "The payment could not be completed.",
  "suggestedNextAction": "Retry"
}
```

### Duplicate Payment Error

```json
{
  "errorCode": "DUPLICATE_PAYMENT",
  "errorMessage": "A successful payment already exists for this order.",
  "suggestedNextAction": "DoNotRetry"
}
```

---

## Suggested Successful Response

```json
{
  "orderId": "f5ee5561-bb0c-4af2-aebd-2e6f43615a03",
  "paymentMethod": "UPI",
  "transactionReference": "TXN-20260715-100001",
  "timestamp": "2026-07-15T09:30:00+05:30"
}
```

---

## Suggested Core Entities

### Order

```text
Id
CustomerId
TotalAmount
Status
CreatedAt
ConfirmedAt
```

### PaymentTransaction

```text
Id
TransactionReference
OrderId
PaymentMethod
Amount
Status
FailureCode
FailureMessage
CreatedAt
CompletedAt
OriginalTransactionId
```

### PaymentAuditLog

```text
Id
OrderId
TransactionId
Activity
Timestamp
SafeMetadata
```

---

## Design Recommendation

Use the **Strategy Pattern** for payment processing.

Each payment method should implement a common payment-processing contract.

Examples:

- Credit Card Payment Strategy
- UPI Payment Strategy
- Net Banking Payment Strategy
- Wallet Payment Strategy
- Cash on Delivery Strategy

This allows new payment methods to be added without changing the existing payment-processing service.
