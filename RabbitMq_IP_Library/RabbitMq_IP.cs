using Bukimedia.PrestaSharp.Factories;
using RabbitMq_IP_Library.ServiceReference1;
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

namespace RabbitMq_IP_Library
{
    //  This is the core class that integrate the 3 systems by rabbitmq messaging solution.
    //  Every integration flow is identified by a UUID, that is used in rabbitmq.
    public static class RabbitMq_IP
    {
        /// <summary>
        /// RABBITMQ SENDERS AND RECEIVERS
        /// </summary>
        
        //a2756b7c-013c-49e9-a4a2-80100387e2f6
        //NEW HMS CUSTOMER SENDER
        public static Action Notify_new_hms_customers(
          string BASE_URL, string ACCOUNT, string PASSWORD, string SENDER)
        {
            return new Action(() => {
                ////
                ////
                //This task is a endless loop where a call is made to the prestashop api to check
                //for new data. When new data found a new message is sended to rabbitmq.
                //
                //The check must first make a local list, sending a initializing message white 
                //all the data given by the prestashop api. Then in the loop check for new data by checking 
                //white the local list. Local list will reflect the actual data status of prestashop

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
                    RabbitMq_IP_Library.Message_new_customer[] messages = new RabbitMq_IP_Library.Message_new_customer[check_list.Count()];
                    if (check_list.Count() != 0 || actual_list.Count() == 0)
                    {

                        Console.WriteLine("New customers count: " + check_list.Count());
                        int index = 0;
                        foreach (Bukimedia.PrestaSharp.Entities.customer c in check_list)
                        {

                            //Message
                            Bukimedia.PrestaSharp.Entities.address customer_address = getAdres((long)c.id, BASE_URL, ACCOUNT, PASSWORD);
                            RabbitMq_IP_Library.Message_new_customer message = new RabbitMq_IP_Library.Message_new_customer()
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
                        factory.HostName = "localhost";

                        IConnection conn = factory.CreateConnection();
                        IModel channel = conn.CreateModel();

                        channel.ExchangeDeclare(
                            exchange: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
                            type: ExchangeType.Direct);
                        channel.QueueDeclare(
                            queue: "a2756b7c-013c-49e9-a4a2-80100387e2f6_queue",
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);
                        channel.QueueBind(
                            queue: "a2756b7c-013c-49e9-a4a2-80100387e2f6_queue",
                            exchange: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
                            routingKey: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange");

                        string messageBody = JsonConvert.SerializeObject(messages);
                        byte[] messageBodyBytes = Encoding.UTF8.GetBytes(messageBody);

                        //Send
                        channel.BasicPublish(
                            exchange: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
                            routingKey: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
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

        //ffc7217a-7a97-42d2-9ba3-09a46209ce20
        //NEW HMS CUSTOMER RECEIVER
        public static Action New_hms_customer_notified(
            string BASE_URL, string ACCOUNT, string PASSWORD, string RECEIVER)
        {
            return new Action(() =>
            {
                //WAIT FOR NEW MESSAGES
                var factory = new ConnectionFactory();
                factory.UserName = "guest";
                factory.Password = "guest";
                factory.HostName = "10.3.51.37";

                var conn = factory.CreateConnection();
                var channel = conn.CreateModel();

                channel.ExchangeDeclare(
                    exchange: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
                    type: ExchangeType.Direct);
                channel.QueueDeclare(
                    queue: "a2756b7c-013c-49e9-a4a2-80100387e2f6_queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                channel.QueueBind(
                    queue: "a2756b7c-013c-49e9-a4a2-80100387e2f6_queue",
                    exchange: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange",
                    routingKey: "a2756b7c-013c-49e9-a4a2-80100387e2f6_exchange");

                var suitecrm_consumer = new EventingBasicConsumer(channel);
                suitecrm_consumer.Received += async (IModel, ea) =>
                {
                    //PULL DATA FROM MESSAGE
                    var bodyString = Encoding.UTF8.GetString(ea.Body);
                    List<Message_new_customer> message_list = JsonConvert.DeserializeObject<List<Message_new_customer>>(bodyString.ToString());
                    await System.Threading.Tasks.Task.Run(() => New_hms_customer_received(
                                                                    message_list, 
                                                                    BASE_URL, ACCOUNT, PASSWORD, RECEIVER));

                    //ACK
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                channel.BasicConsume(queue: "a2756b7c-013c-49e9-a4a2-80100387e2f6_queue", autoAck: false, consumer: suitecrm_consumer);


            });
        }
        
        /// <summary>
        /// RABBITMQ EVENT CONSUMER HANDLERS
        /// </summary>

        //This function is used to preces a received message from rabbitmq.
        private static void New_hms_customer_received(
            List<Message_new_customer> message_list, 
            string BASE_URL, string ACCOUNT, string PASSWORD, string RECEIVER)
        {
            Console.WriteLine("\n \n!NEW message received" + "\n");

            



            //GET SUITECRM USERS
            ServiceReference1.sugarsoapPortTypeClient soap = new ServiceReference1.sugarsoapPortTypeClient("sugarsoapPort");
            ServiceReference1.user_auth user = new ServiceReference1.user_auth();

            user.user_name = ACCOUNT;
            user.password = PASSWORD;

            ServiceReference1.name_value[] loginList = new ServiceReference1.name_value[0];
            ServiceReference1.entry_value result_login = soap.login(user, "SOAP_RABBITMQ", loginList);

            string sessionId = result_login.id;

            ServiceReference1.get_entry_list_result_version2 suiteCrm_all_accounts = soap.get_entry_list(
                sessionId,
                "Accounts",
                "",
                "",
                0,
                new[] { "id", "name" },
                null,
                99999,
                0,
                false);



            //PROCES MESSAGE
            foreach (Message_new_customer m in message_list)
            {
                int index = 0;
                bool contain = false;
                foreach (entry_value var in suiteCrm_all_accounts.entry_list)
                {

                    string soapC_name = GetValueFromNameValueList("name", var.name_value_list);
                    string soapC_id = GetValueFromNameValueList("id", var.name_value_list);

                    //Console.WriteLine(name);


                    //UPDATE
                    if (soapC_name.Contains(m.firstname) && soapC_name.Contains(m.lastname))
                    {
                        contain = true;


                        soapC_name = m.firstname + " " + m.lastname;
                        string soapC_billingA1 = m.address1;
                        string soapC_billingA2 = m.address2;
                        string soapC_billingCity = m.city;
                        string soapC_billingState = m.city;
                        string soapC_billingPostalcode = m.postcode;
                        string soapC_billingCountry = m.city;
                        string soapC_billingPhoneM = m.phone_mobile;
                        string soapC_billingPhone = m.phone;
                        string soapC_billingEmail = m.email;

                        NameValueCollection fieldListCollection = new NameValueCollection();
                        //to update a record, you will nee to pass in a record id as commented below
                        fieldListCollection.Add("id", soapC_id);
                        fieldListCollection.Add("name", soapC_name);
                        fieldListCollection.Add("billing_address_street", soapC_billingA1);
                        fieldListCollection.Add("billing_address_street_2", soapC_billingA2);
                        fieldListCollection.Add("billing_address_city", soapC_billingCity);
                        fieldListCollection.Add("billing_address_state", soapC_billingState);
                        fieldListCollection.Add("billing_address_postalcode", soapC_billingPostalcode);
                        fieldListCollection.Add("billing_address_country", soapC_billingCountry);
                        fieldListCollection.Add("phone_office", soapC_billingPhoneM);
                        fieldListCollection.Add("phone_fax", soapC_billingPhone);
                        fieldListCollection.Add("email1", soapC_billingEmail);

                        //this is just a trick to avoid having to manually specify index values for name_value[]
                        ServiceReference1.name_value[] fieldList = new ServiceReference1.name_value[fieldListCollection.Count];

                        int count = 0;
                        foreach (string name in fieldListCollection)
                        {
                            foreach (string value in fieldListCollection.GetValues(name))
                            {
                                ServiceReference1.name_value field = new ServiceReference1.name_value();
                                field.name = name; field.value = value;
                                fieldList[count] = field;
                            }
                            count++;
                        }

                        try
                        {
                            ServiceReference1.new_set_entry_result result_insert = soap.set_entry(sessionId, "Accounts", fieldList);
                            string RecordID = result_insert.id;

                            //show record id to user
                            Console.WriteLine("=> customer updated, id: " + RecordID + "\n");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine(ex.Source);
                        }

                    }
                }

                //CREATE
                if (contain == false)
                {
                    Console.WriteLine("New customer");


                    string soapC_name = m.firstname + m.lastname;
                    string soapC_billingA1 = m.address1;
                    string soapC_billingA2 = m.address2;
                    string soapC_billingCity = m.city;
                    string soapC_billingState = m.city;
                    string soapC_billingPostalcode = m.postcode;
                    string soapC_billingCountry = m.city;
                    string soapC_billingPhoneM = m.phone_mobile;
                    string soapC_billingPhone = m.phone;
                    string soapC_billingEmail = m.email;


                    NameValueCollection fieldListCollection = new NameValueCollection();

                    fieldListCollection.Add("name", soapC_name);
                    fieldListCollection.Add("billing_address_street", soapC_billingA1);
                    fieldListCollection.Add("billing_address_street_2", soapC_billingA2);
                    fieldListCollection.Add("billing_address_city", soapC_billingCity);
                    fieldListCollection.Add("billing_address_state", soapC_billingState);
                    fieldListCollection.Add("billing_address_postalcode", soapC_billingPostalcode);
                    fieldListCollection.Add("billing_address_country", soapC_billingCountry);
                    fieldListCollection.Add("phone_office", soapC_billingPhoneM);
                    fieldListCollection.Add("phone_fax", soapC_billingPhone);
                    fieldListCollection.Add("email1", soapC_billingEmail);

                    ServiceReference1.name_value[] fieldList = new ServiceReference1.name_value[fieldListCollection.Count];
                    int count = 0;
                    foreach (string name in fieldListCollection)
                    {
                        foreach (string value in fieldListCollection.GetValues(name))
                        {
                            ServiceReference1.name_value field = new ServiceReference1.name_value();
                            field.name = name; field.value = value;
                            fieldList[count] = field;
                        }
                        count++;
                    }

                    try
                    {
                        ServiceReference1.new_set_entry_result result_insert = soap.set_entry(sessionId, "Accounts", fieldList);
                        string RecordID = result_insert.id;

                        //show record id to user
                        Console.WriteLine("=> customer added, id: " + RecordID + "\n");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.Source);
                    }
                }

            }



        }

        /// <summary>
        /// ...
        /// </summary>

        //This function is used to get values from soap result list.
        private static string GetValueFromNameValueList(
            string key, IEnumerable<name_value> nameValues)
        {
            if (nameValues.Where(nv => nv.name == key).ToArray().Count() != 0)
                return nameValues.Where(nv => nv.name == key).ToArray()[0].value;
            return "";
        }

        //This function is used to get the adres of a costumer in prestashop.
        public static Bukimedia.PrestaSharp.Entities.address getAdres(
            long customer_id, string BASE_URL, string ACCOUNT, string PASSWORD)
        {
            AddressFactory addresses = new AddressFactory(BASE_URL, ACCOUNT, PASSWORD);
            List<Bukimedia.PrestaSharp.Entities.address> all = addresses.GetAll();

            foreach (Bukimedia.PrestaSharp.Entities.address a in all)
            {
                if (a.id_customer == customer_id)
                {
                    return a;
                }

            }
            return null;
        }
    }
}
