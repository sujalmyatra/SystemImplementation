## Inventory and Warehouse Management System

The Inventory and Warehouse Management System is designed to manage products, warehouses, suppliers, stock availability, and stock movement between different parties. The system keeps track of how much quantity of each product is available in each warehouse and supports stock transfers between warehouses, suppliers, and warehouses.

### Main Entities

The system will have the following main entities:

1. **Product**
   A product represents an item that is stored, purchased, supplied, transferred, or sold.

2. **Warehouse**
   A warehouse represents a storage location where products are kept. The same product can be available in multiple warehouses with different quantities.

3. **Supplier**
   A supplier provides products to warehouses and may also receive returned stock from warehouses.

4. **Stock Movement**
   A stock movement represents every stock-related transaction in the system. It tracks stock coming in, going out, or being transferred.

### Stock Movement Types

The system should support the following types of stock movements:

1. **Supplier to Warehouse**
   When a supplier provides products to a warehouse, stock quantity increases in that warehouse.

2. **Warehouse to Supplier**
   When products are returned from a warehouse to a supplier, stock quantity decreases from that warehouse.

3. **Warehouse to Warehouse**
   When products are transferred from one warehouse to another, stock decreases from the source warehouse and increases in the destination warehouse.

### Stock Transfer Rule

When transferring stock from one warehouse to another, the source warehouse must have at least the requested quantity of that product. If the available stock is less than the transfer quantity, the system should not allow the transfer.

Example:

If Warehouse A has 50 units of Product X, then only 50 or fewer units can be transferred from Warehouse A to another warehouse.

### Stock Threshold Rules

The system should monitor stock levels for each product in each warehouse.

If the product quantity is less than 20, the system should trigger a **Low Stock** alert.

If the product quantity is 0, the system should show the product as **Out of Stock** or **No Stock**.

Example:

Quantity = 15 → Low Stock
Quantity = 0 → No Stock
Quantity = 30 → Available Stock

### Reports

The system should provide the following reports:

#### 1. Stock Value Report

This report calculates the total stock value of each product based on its quantity.

Formula:

Product Stock Value = Product Price × Available Quantity

Example:

If Product A has a price of ₹100 and quantity is 50, then stock value is:

₹100 × 50 = ₹5000

#### 2. Warehouse-wise Product Stock Report

Since the same product can be stored in different warehouses, the system should show product quantity warehouse-wise.

Example:

Product A:

* Warehouse 1: 30 units
* Warehouse 2: 50 units
* Warehouse 3: 10 units

#### 3. Fast-Moving Product Report

This report shows products that are frequently moved or sold. Products with high stock movement or high sales quantity should be considered fast-moving products.

#### 4. Slow-Moving Product Report

This report shows products that are not sold or moved frequently.

A product should be considered slow-moving if:

* It has not been sold in the last month, or
* It was sold fewer than 5 times in the last month
5. Low-Stock Product Report

This report shows products whose available quantity is below the minimum stock level in a particular warehouse.

A stock record should be considered low-stock when:

Quantity is greater than 0
and
Quantity is less than 20

Example:

Product A — Warehouse 1: 12 units
Product B — Warehouse 2: 5 units
Product C — Warehouse 1: 18 units

The report should show:

Product Name
Warehouse Name
Available Quantity
Stock Status

Products with zero quantity should not appear because they belong to the out-of-stock report.

#### 5. Low-Stock Product Report

This report shows products whose available quantity is below the minimum stock level in a particular warehouse.

A stock record should be considered low-stock when:

Quantity is greater than 0
and
Quantity is less than 20

Example:

Product A — Warehouse 1: 12 units
Product B — Warehouse 2: 5 units
Product C — Warehouse 1: 18 units

The report should show:

Product Name
Warehouse Name
Available Quantity
Stock Status

Products with zero quantity should not appear because they belong to the out-of-stock report.
#### 6. Warehouse Stock Value Report

This report calculates the total value of stock stored in each warehouse.

Formula:

Warehouse Stock Value =
Sum of Product Unit Price × Product Quantity

Example:

Warehouse 1 contains:

Product A:
Price = ₹100
Quantity = 20
Value = ₹2,000

Product B:
Price = ₹500
Quantity = 10
Value = ₹5,000

Total warehouse stock value:

₹2,000 + ₹5,000 = ₹7,000

The report should show:

Warehouse Name
Total Product Quantity
Total Stock Value

Expected result:

Warehouse 1 — 30 units — ₹7,000
Warehouse 2 — 50 units — ₹12,000
#### 7. Supplier-wise Product Supply Report

This report shows which products were supplied by each supplier during the previous month.

Only movements of this type should be considered:

MovementType.SupplierToWarehouse

The report should show:

Supplier Name
Product Name
Number of Supply Movements
Total Supplied Quantity

Example:

Supplier A:
    Product A — 3 supply movements — 100 units
    Product B — 2 supply movements — 50 units

Supplier B:
    Product A — 1 supply movement — 20 units

Soft-deleted stock movements should not be included.

#### 8. Product Transfer Report

This report shows products transferred from one warehouse to another during a given date range.

Only movements of this type should be considered:

MovementType.WarehouseToWarehouse

The report should show:

Product Name
From Warehouse
To Warehouse
Number of Transfers
Total Transferred Quantity

Example:

Product A:
Warehouse 1 → Warehouse 2
Transfer Count: 3
Transferred Quantity: 70 units

If multiple transfers occur for the same product between the same two warehouses, their quantities should be combined.

Example data:

Product A: Warehouse 1 → Warehouse 2: 20 units
Product A: Warehouse 1 → Warehouse 2: 30 units
Product A: Warehouse 1 → Warehouse 3: 10 units

Expected result:

Product A — Warehouse 1 → Warehouse 2 — 2 transfers — 50 units
Product A — Warehouse 1 → Warehouse 3 — 1 transfer — 10 units
#### 9. Products Without Recent Movement Report

This report shows products that have not had any stock movement during the last 60 days.

A product should appear when:

It has no stock movement at all
or
Its latest stock movement occurred more than 60 days ago

The report should show:

Product Name
Current Total Quantity
Last Movement Date
Inactive Days

Example:

Product A — 50 units — Last movement: 70 days ago
Product B — 0 units — No movement recorded

When a product has never had a movement, show:

Last Movement Date = "No Movement"

This report must include products with zero movement records, so beginning only from stockMovements would not be enough.

#### 10. Warehouse Inward and Outward Movement Report

This report shows the total quantity entering and leaving each warehouse during the current month.

Inward movements

A warehouse receives stock when:

SupplierToWarehouse:
ToWarehouseId is the warehouse

WarehouseToWarehouse:
ToWarehouseId is the warehouse
Outward movements

Stock leaves a warehouse when:

WarehouseToWarehouse:
FromWarehouseId is the warehouse

WarehouseToSupplier:
FromWarehouseId is the warehouse

The report should show:

Warehouse Name
Total Inward Quantity
Total Outward Quantity
Net Quantity Change

Formula:

Net Quantity Change =
Total Inward Quantity - Total Outward Quantity

Example:

Warehouse 1:
Inward Quantity = 200
Outward Quantity = 120
Net Change = 80

Expected result:

Warehouse 1 — Inward: 200 — Outward: 120 — Net: +80
Warehouse 2 — Inward: 50 — Outward: 90 — Net: -40

These five scenarios practice different thinking patterns:

Report	Main challenge
Low-stock report	Filtering product-warehouse stock
Warehouse stock value	Grouping and multiplication
Supplier supply report	Date, type filtering, and grouping
Product transfer report	Grouping by product, source, and destination
No recent movement	Including products with zero movements
Inward/outward report	Separate calculations for the same warehouse
### Soft Delete for Stock Movements

Stock movements should not be permanently deleted from the database. Instead, the system should use soft delete.

When a stock movement is deleted, it should be marked as deleted using a field like `IsDeleted = true`.

This helps preserve historical stock transaction records for auditing and reporting purposes.

### Overall System Goal

The main goal of this system is to accurately manage product stock across multiple warehouses, track stock movement between suppliers and warehouses, prevent invalid stock transfers, monitor low-stock and out-of-stock products, and generate useful inventory reports for business decision-making.
