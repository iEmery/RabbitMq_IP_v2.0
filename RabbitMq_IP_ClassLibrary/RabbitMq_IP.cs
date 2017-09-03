using Bukimedia.PrestaSharp.Factories;
using RabbitMq_IP_ClassLibrary.ServiceReference1;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMq_IP_ClassLibrary
{
    class RabbitMq_IP
    {

        //NEW HMS CUSTOMER SENDER
        public static Action Notify_new_hms_customers(
          string BASE_URL, string ACCOUNT, string PASSWORD, string SENDER)
        {
            return new Action(() => {
                ///

                Console.WriteLine("!New HMS customers notifier strarted. @Thread: " + Thread.CurrentThread.ManagedThreadId + "\n");

                //Notify new customers
                List<Bukimedia.PrestaSharp.Entities.customer> actual_list = new List<Bukimedia.PrestaSharp.Entities.customer>();
                while (true)
                {
                    //Get all users from HMS
                    CustomerFactory customers = new CustomerFactory(BASE_URL, ACCOUNT, PASSWORD);
                    List<Bukimedia.PrestaSharp.Entities.customer> all_presta_customers = customers.GetAll();

                    //Check for new data
                    IEnumerable<Bukimedia.PrestaSharp.Entities.customer> check_list = new List<Bukimedia.PrestaSharp.Entities.customer>();
                    if (actual_list.Count == 0)
                    {
                        Console.WriteLine("Initialize message will be sendend");
                        check_list = all_presta_customers;
                    }
                    else
                    {
                        foreach (Bukimedia.PrestaSharp.Entities.customer c in all_presta_customers)
                        {
                            bool contain = false;
                            foreach (Bukimedia.PrestaSharp.Entities.customer c1 in actual_list)
                            {
                                if (c.id == c1.id)
                                {
                                    contain = true;
                                    break;
                                }
                            }
                            if (contain == false)
                            {
                                List<Bukimedia.PrestaSharp.Entities.customer> tmp1 = check_list.ToList<Bukimedia.PrestaSharp.Entities.customer>();
                                tmp1.Add(c);
                                check_list = tmp1.ToList();
                            }

                        }
                    }


                    //Format messages from new data
                    Messages_ClassLibrary.Message_new_customer[] messages = new Messages_ClassLibrary.Message_new_customer[check_list.Count()];
                    if (check_list.Count() != 0 || actual_list.Count() == 0)
                    {

                        Console.WriteLine("New customers count: " + check_list.Count());
                        int index = 0;
                        foreach (Bukimedia.PrestaSharp.Entities.customer c in check_list)
                        {

                            //Message
                            Bukimedia.PrestaSharp.Entities.address customer_address = getAdres((long)c.id, BASE_URL, ACCOUNT, PASSWORD);
                            Messages_ClassLibrary.Message_new_customer message = new Messages_ClassLibrary.Message_new_customer()
                            {
                                firstname = c.firstname,
                                lastname = c.lastname,
                                email = c.email,
                                birthday = c.birthday,
                                address1 = (customer_address != null && customer_address.address1 != null) ? customer_address.address1.ToString() : "",
                                address2 = (customer_address != null && customer_address.address2 != null) ? customer_address.address2.ToString() : "",
                                postcode = (customer_address != null && customer_address.postcode != null) ? customer_address.postcode.ToString() : "",
                                city = (customer_address != null && customer_address.city != null) ? customer_address.city.ToString() : "",
                                phone = (customer_address != null && customer_address.phone != null) ? customer_address.phone.ToString() : "",
                                phone_mobile = (customer_address != null && customer_address.phone_mobile != null) ? customer_address.phone_mobile.ToString() : "",
                            };
                            messages[index] = message;
                            index++;

                        }

                    }

                    //Send new data
                    if (messages.Count() != 0)
                    {
                        //RabbitMq
                        ConnectionFactory factory = new ConnectionFactory();
                        factory.UserName = "guest";
                        factory.Password = "guest";
                        factory.HostName = "10.3.51.37";

                        IConnection conn = factory.CreateConnection();
                        IModel channel = conn.CreateModel();

                        channel.ExchangeDeclare(
                            exchange: "new_hms_customer_exchange",
                            type: ExchangeType.Direct);
                        channel.QueueDeclare(
                            queue: "hms_crm_customer_queue",
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                        channel.QueueBind(
                            queue: "hms_crm_customer_queue",
                            exchange: "new_hms_customer_exchange",
                            routingKey: "new_hms_customer_exchange");

                        string messageBody = JsonConvert.SerializeObject(messages);
                        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(messageBody);

                        //Send
                        channel.BasicPublish(
                            exchange: "new_hms_customer_exchange",
                            routingKey: "new_hms_customer_exchange",
                            basicProperties: null,
                            body: messageBodyBytes);

                        Console.WriteLine("new data sended. @Thread: " + Thread.CurrentThread.ManagedThreadId);
                        channel.Dispose();
                        conn.Dispose();
                    }

                    actual_list = all_presta_customers;

                }


                ///
            });
        }

    }
}
