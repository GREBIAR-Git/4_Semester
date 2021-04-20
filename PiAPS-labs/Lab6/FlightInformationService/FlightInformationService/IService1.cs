﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace FlightInformationService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string InformationSpecifiedRoute(string startPoint, string destinationPoint);
        [OperationContract]
        string NumberTickets(string numberFlight);
        [OperationContract]
        string BookTickets(string numberFlight);


        // TODO: Добавьте здесь операции служб
    }
}