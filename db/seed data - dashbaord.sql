-- kpis
-- Values computed from "seed data - invoice.sql" / "seed data - customer.sql":
--   Total Billed = sum of all invoice amounts
--   Collected    = sum of amounts where status = 'paid'
--   Outstanding  = sum of amounts where status = 'pending'
--   Customers    = row count of seeded customers
INSERT INTO [ledgerly-dashbaord].[dbo].[kpis] (ID, kpiname, kpivalue) VALUES
(1, 'Total Billed', 149664),
(2, 'Collected', 30835),
(3, 'Outstanding', 118829),
(4, 'Customers', 9);

-- Revenue
-- One row per month, Jan-Aug 2026 only (invoices don't extend past August),
-- value = sum of 'paid' invoice amounts whose date falls in that month/year.
INSERT INTO [ledgerly-dashbaord].[dbo].[revenue] (month, year, revenue) VALUES
('Jan', 2026, 8545),
('Feb', 2026, 8945),
('Mar', 2026, 1000),
('Apr', 2026, 3040),
('May', 2026, 4800),
('Jun', 2026, 0),
('Jul', 2026, 0),
('Aug', 2026, 4505);
