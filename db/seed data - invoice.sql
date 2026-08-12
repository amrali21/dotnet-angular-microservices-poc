-- Invoices
-- Spread across every month of 2026 so month-over-month charts have data for the whole year.
INSERT INTO [ledgerly-invoice].[dbo].invoices ( customer_id, amount, status, date) VALUES
(  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8545,      'paid',    '2026-01-14'),
(  '3958dc9e-742f-4377-85e9-fec4b6a6442a', 50,        'pending', '2026-02-19'),
(  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8945,      'paid',    '2026-03-03'),
(  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 8942,      'pending', '2026-04-18'),
(  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 1000,      'paid',    '2026-05-05'),
(   '3958dc9e-742f-4377-85e9-fec4b6a6442a', 20348,     'pending', '2026-06-14'),
(   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 3040,      'paid',    '2026-07-29'),
(   '50ca3e18-62cd-11ee-8c99-0242ac120002', 44800,     'paid',    '2026-08-10'),
(   '76d65c26-f784-44a2-ac19-586678f7c2f2', 34577,     'pending', '2026-09-05'),
(   '126eed9c-c90c-4ef6-a4a8-fcf7408d3c66', 54246,     'pending', '2026-10-16'),
(   'd6e15727-9fe1-4961-8c5b-ea44a9bd81aa', 666,       'pending', '2026-11-27'),
(   '50ca3e18-62cd-11ee-8c99-0242ac120002', 32545,     'paid',    '2026-12-09'),
(   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 1250,      'paid',    '2026-12-17')