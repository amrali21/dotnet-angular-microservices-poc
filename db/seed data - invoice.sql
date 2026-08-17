-- Invoices
-- Spread across Jan-Aug 2026 only (nothing past "today"); the two big paid
-- invoices (were 44800/32545) are scaled down to 4800/3255 so no single
-- month's revenue towers over the rest.
INSERT INTO [ledgerly-invoice].[dbo].invoices ( customer_id, amount, status, date) VALUES
(  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8545,      'paid',    '2026-01-12'),
(  '3958dc9e-742f-4377-85e9-fec4b6a6442a', 50,        'pending', '2026-01-25'),
(  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8945,      'paid',    '2026-02-09'),
(  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 8942,      'pending', '2026-02-21'),
(  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 1000,      'paid',    '2026-03-05'),
(   '3958dc9e-742f-4377-85e9-fec4b6a6442a', 20348,     'pending', '2026-03-19'),
(   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 3040,      'paid',    '2026-04-08'),
(   '50ca3e18-62cd-11ee-8c99-0242ac120002', 4800,      'paid',    '2026-05-03'),
(   '76d65c26-f784-44a2-ac19-586678f7c2f2', 34577,     'pending', '2026-06-11'),
(   '126eed9c-c90c-4ef6-a4a8-fcf7408d3c66', 54246,     'pending', '2026-06-24'),
(   'd6e15727-9fe1-4961-8c5b-ea44a9bd81aa', 666,       'pending', '2026-07-14'),
(   '50ca3e18-62cd-11ee-8c99-0242ac120002', 3255,      'paid',    '2026-08-02'),
(   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 1250,      'paid',    '2026-08-09')