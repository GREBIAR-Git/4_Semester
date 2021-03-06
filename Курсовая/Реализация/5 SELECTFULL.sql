CALL search_product("Орёл","Samsung","Монитор");
CALL search_model_characteristics("Ryzen 7 3800XT");
CALL search_models_in_city("Орёл");
CALL discount_model(0.10,'S24F354FHI');
CALL viewing_products(1);
CALL types_manufacturers(4);

-- Количесво филиалов в городе
SELECT c.name Город, COUNT(*) Филиалов
FROM city c
JOIN branch b USING(city_id)
GROUP BY c.name;

-- Чистая прибыль от покупки
SELECT s.sale_id ID_Покупки, SUM(p.price*sp.quantity) Прибыль,
SUM((p.price - p.price_purchases)*sp.quantity) Чистая_прибыль
FROM sale s
JOIN sale_product sp USING(sale_id)
JOIN product p USING(product_id)
GROUP BY sale_id;

-- Сотрудникои которы получат премию
SELECT CONCAT(se.lastname,' ',LEFT(se.firstname,1),IF(se.patronymic is NULL,'.',CONCAT('.',LEFT(se.patronymic,1),'.'))) Фамилия_и_Инициалы,
SUM((p.price)*sp.quantity) Продал_на
FROM product p
JOIN sale_product sp USING(product_id)
JOIN sale sa USING(sale_id)
JOIN seller se USING(seller_id)
WHERE MONTH(sa.date) = MONTH(curdate()) AND YEAR(sa.date) = YEAR(curdate())
GROUP BY se.seller_id
HAVING SUM((p.price)*sp.quantity)>100000
ORDER BY Фамилия_и_Инициалы;

-- Сумма прибыли с покупок за последний месяц во всей сети компьютерных магазинов
SELECT SUM((p.price)*sp.quantity) Сумма
FROM sale s
JOIN sale_product sp USING(sale_id)
JOIN product p USING(product_id)
WHERE month(s.date) = month(curdate());

-- Сумма прибыли с покупок за последний год во всей сети компьютерных магазинов
SELECT SUM((p.price)*sp.quantity) Сумма
FROM sale s
JOIN sale_product sp USING(sale_id)
JOIN product p USING(product_id)
WHERE (year(s.date) = year(curdate()));

-- Сумма прибыли с покупок за месяц в филиалах по отдельности
SELECT c.name Город,CONCAT(b.street,', ', b.house) Адрес,  IF(SUM((p.price)*sp.quantity) is NULL,0,SUM((p.price)*sp.quantity)) Прибыль_за_месяц
FROM sale s
JOIN seller USING(seller_id)
JOIN sale_product sp USING(sale_id)
JOIN product p USING(product_id)
RIGHT JOIN branch b USING (branch_id)
JOIN city c USING(city_id)
WHERE month(s.date) = month(curdate()) or s.date is NULL
GROUP BY b.branch_id;

-- чек
SELECT sa.sale_id, CONCAT(co.name,', г.', ci.name,', ул.', b.street, ', д.', b.house) Адрес, 
CONCAT(se.lastname,' ',LEFT(se.firstname,1),IF(se.patronymic is NULL,'.',CONCAT('.',LEFT(se.patronymic,1),'.'))) Продавец,
sa.date Дата, 
SUM(sp.quantity) количество_товара,
SUM((p.price)*sp.quantity) итоговая_стоимость_заказа
FROM country co
JOIN city ci USING(country_id)
JOIN branch b USING(city_id)
JOIN seller se USING(branch_id)
JOIN sale sa USING(seller_id)
JOIN sale_product sp USING(sale_id)
JOIN product p USING(product_id)
JOIN type t USING(type_id)
GROUP BY sale_id;

SELECT p.name модель, m.name Фирма, CONCAT(ph.value,' ' , ch.measure_units) Диагональ, bp.quantity Количество
FROM branch b
JOIN branch_product bp USING (branch_id)
JOIN product p USING (product_id)
JOIN manufacturer m USING (manufacturer_id)
JOIN product_characteristics ph USING (product_id)
JOIN characteristics ch USING (characteristics_id)
WHERE ch.name='Диагональ' AND b.branch_id=1 AND m.name='LG' AND ph.value > 15
