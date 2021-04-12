# Лабораторные 2-3
Клиент-серверная архитектура
Когда к серверу подключается клиент, на сервер создает поток для прослушивания клиента, а на клиенте создается поток для прослушивания сервера.
Сервер в папке Server, клиент в папке ClientInterface

Ответы на контрольные вопросы (лабораторная 2):

	1. Перечислите основные разновидности клиент-серверной архитектуры.
Однозвенная архитектура, двухзвенная архитектура, трехзвенная архитектура
	
	2. Опишите стек протоколов используемых при организации клиент-серверной архитектуры. 
Протоколы TCP/IP, протоколы OSI, а также различные фирменные архитектуры, вроде SNA. Назначение всего этого программного обеспечения поддержки (протоколов и операционной системы) заключается в предоставлении базы для распределенных приложений.
	
	3. Какие компоненты платформы .NET используются для прослушивания входящих подключений на определённом порте?
System.Net.Sockets, System.Net
	
	4. Что такое потоки в программировании?
Поток — это по сути последовательность инструкций, которые выполняются параллельно с другими потоками. Каждая программа создает по меньшей мере один поток.
	
	5. Как реализуются потоки в языке C#?
using System.Threading; - библиотека для потоков
Thread myThread = new Thread(new ThreadStart(myFunction)); - создание идентификатора потока myThread, выполняющего функцию myFunction
myThread.Start(); - запуск потока myThread

Ответы на контрольные вопросы (лабораторная 3):
	
	1. Опишите механизм построения графического пользовательского интерфейса в платформе .NET.
В платформе .NET для реализации графического интерфейса используются разные технологии, такие как Windows Forms и Windows Presentation Foundation (WPF). Технология Windows Forms включает множество типов (классы, структуры, перечисления, делегаты), которые объединены в два основных пространства имен System.Windows.Forms (для реализации элементов интерфейса) и System.Drawing (для рисования в клиентской области). Основными элементами графического интерфейса являются специальные классы, называемые элементами управления (ЭУ), которые обладают двумя особенностями: 1) реализуют работу с различными типами окон ОС Windows; 2) поддерживают работу в двух режимах: а) режим проектирования (design mode), в котором с ними работает среда разработки; б) режим выполнения (run mode), в котором выполняется взаимодействие пользователей с ЭУ. Базовым классом для всех элементов управления является класс Control, реализующий самую базовую функциональность. Класс Control задает важные свойства, методы и события, наследуемые всеми его потомками. Все классы элементов управления (ЭУ) являются наследниками класса Control. Базовый класс Control содержит достаточно большой интерфейс, который доступен во всех производных классах.
	
	2. Опишите принцип работы делегатов в языке C#.
Делегаты представляют такие объекты, которые указывают на методы. То есть делегаты - это указатели на методы и с помощью делегатов мы можем вызвать данные методы.
Делегаты C# обладают следующими свойствами:
-позволяют обрабатывать методы в качестве аргумента;
-могут быть связаны вместе;
-несколько методов могут быть вызваны по одному событию;
-тип делегата определяется его именем;
-не зависят от класса объекта, на который ссылается;
-сигнатура метода должна совпадать с сигнатурой делегата.
Синтаксис: 
Шаг 1 — Объявление modifier delegate ReturnType DelegateName ([Parameter_1])
Шаг 2 — Инициализация Делегат инициализируется путём передачи ему имени метода в качестве аргумента. DelegateName DelgObjectName = new DelegateName(MethodName)
Шаг 3 — Вызов Вызываем созданный делегат с указанием параметров, если это необходимо. DelegateObjectName([Parameter_1]);
	
	3. Опишите механизм обновления пользовательского интерфейса из параллельного потока.
Сначала нужно использовать Dispatcer.Invoke чтобы изменить пользовательский интерфейс из другого потока. Для этого нужно использовать события, затем можно зарегистрировать на события в основном классе и отправить изменения в пользовательский интерфейс