
--Users
INSERT INTO [nextjs-test].[dbo].[users] (id, name, email, password)
VALUES
('410544b2-4001-4271-9855-fec4b6a6442a', 'User', 'user@nextmail.com', '123456');

-- Customers
INSERT INTO  [customers] ([id], [name], [email], [image_url]) VALUES
('126eed9c-c90c-4ef6-a4a8-fcf7408d3c66', 'Emil Kowalski', 'emil@kowalski.com', '/customers/emil-kowalski.png'),
('13D07535-C59E-4157-A011-F8D2EF4E0CBB', 'Balazs Orban', 'balazs@orban.com', '/customers/balazs-orban.png'),
('3958dc9e-712f-4377-85e9-fec4b6a6442a', 'Delba de Oliveira', 'delba@oliveira.com', '/customers/delba-de-oliveira.png'),
('3958dc9e-737f-4377-85e9-fec4b6a6442a', 'Hector Simpson', 'hector@simpson.com', '/customers/hector-simpson.png'),
('3958dc9e-742f-4377-85e9-fec4b6a6442a', 'Lee Robinson', 'lee@robinson.com', '/customers/lee-robinson.png'),
('3958dc9e-787f-4377-85e9-fec4b6a6442a', 'Steph Dietz', 'steph@dietz.com', '/customers/steph-dietz.png'),
('50ca3e18-62cd-11ee-8c99-0242ac120002', 'Steven Tey', 'steven@tey.com', '/customers/steven-tey.png'),
('76d65c26-f784-44a2-ac19-586678f7c2f2', 'Michael Novotny', 'michael@novotny.com', '/customers/michael-novotny.png'),
('d6e15727-9fe1-4961-8c5b-ea44a9bd81aa', 'Evil Rabbit', 'evil@rabbit.com', '/customers/evil-rabbit.png');

-- Invoices
INSERT INTO invoices (id, customer_id, amount, status, date) VALUES
(10,  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8545,      'paid',    '2023-06-07'),
(11,  '3958dc9e-742f-4377-85e9-fec4b6a6442a', 50,        'pending', '2023-08-19'),
(12,  '76d65c26-f784-44a2-ac19-586678f7c2f2', 8945,      'paid',    '2023-06-03'),
(13,  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 8942,      'pending', '2023-06-18'),
(15,  '3958dc9e-737f-4377-85e9-fec4b6a6442a', 1000,      'paid',    '2022-06-05'),
(2,   '3958dc9e-742f-4377-85e9-fec4b6a6442a', 20348,     'pending', '2022-11-14'),
(3,   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 3040,      'paid',    '2022-10-29'),
(4,   '50ca3e18-62cd-11ee-8c99-0242ac120002', 44800,     'paid',    '2023-09-10'),
(5,   '76d65c26-f784-44a2-ac19-586678f7c2f2', 34577,     'pending', '2023-08-05'),
(6,   '126eed9c-c90c-4ef6-a4a8-fcf7408d3c66', 54246,     'pending', '2023-07-16'),
(7,   'd6e15727-9fe1-4961-8c5b-ea44a9bd81aa', 666,       'pending', '2023-06-27'),
(8,   '50ca3e18-62cd-11ee-8c99-0242ac120002', 32545,     'paid',    '2023-06-09'),
(9,   '3958dc9e-787f-4377-85e9-fec4b6a6442a', 1250,      'paid',    '2023-06-17'),
(91,  '3958dc9e-742f-4377-85e9-fec4b6a6442a', 10000000,  'paid',    '0001-01-01'),
(911, '13D07535-C59E-4157-A011-F8D2EF4E0CBB', 100000000, 'pending', '0001-01-01');

-- Revenue
INSERT INTO revenue (month, revenue) VALUES
('Apr', 2500),
('Aug', 3700),
('Dec', 4800),
('Feb', 1800),
('Jan', 2000),
('Jul', 3500),
('Jun', 3200),
('Mar', 2200),
('May', 2300),
('Nov', 3000),
('Oct', 2800),
('Sep', 2500);