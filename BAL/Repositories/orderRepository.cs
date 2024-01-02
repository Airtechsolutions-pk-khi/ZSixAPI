﻿using DAL.DBEntities;
using DAL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class orderRepository : BaseRepository
    {

        public orderRepository()
            : base()
        {
            DBContext = new db_a82b87_zsixrestaurantEntities();

        }

        public orderRepository(db_a82b87_zsixrestaurantEntities contextDB)
            : base(contextDB)
        {
            DBContext = contextDB;
        }

        public RspOrdersCustomer GetOrderCustomersV2(int orderID, int customerID)
        {
            var status1 = new OrderStatusChildBLL();
            var status2 = new OrderStatusChildBLL();
            var status3 = new OrderStatusChildBLL();
            var bll = new List<OrdersBLL>();
            var lstOD = new List<OrderDetailBLL>();
            var lstODM = new List<OrderModifiersBLL>();
            var lstODA = new List<OrderAddonsBLL>();
            var oc = new OrderCheckoutBLL();
            var ocustomer = new OrderCustomerBLL();
            var lstOrderStatus = new OrderStatusBLL();
            var rsp = new RspOrdersCustomer();
            try
            {
                var ds = GetCustomerOrder_ADO(customerID);
                var _dsOrders = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0])).ToObject<List<OrdersBLL>>();
                var _dsorderdetail = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1])).ToObject<List<OrderDetailBLL>>();
                var _dsorderdetailmodifier = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[2])).ToObject<List<OrderModifiersBLL>>();
                var _dsOrdercheckout = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[3])).ToObject<List<OrderCheckoutBLL>>();
                var _dsOrderCustomerData = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[4])).ToObject<List<OrderCustomerBLL>>();
                var _dsorderdetailaddon = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject<List<OrderAddonsBLL>>();
                //var _dsLocation = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject<LocationsBLL>();
                //var _dsBrand = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject< List<BrandsBLL>>().FirstOrDefault();


                foreach (var i in _dsOrders)
                {
                    lstOD = new List<OrderDetailBLL>();
                    foreach (var j in _dsorderdetail.Where(x => x.StatusID == 201 && x.OrderID == i.OrderID))
                    {
                        lstODM = new List<OrderModifiersBLL>();
                        foreach (var k in _dsorderdetailmodifier.Where(x => x.StatusID == 201 && x.OrderDetailID == j.OrderDetailID))
                        {
                            lstODM.Add(new OrderModifiersBLL
                            {
                                StatusID = k.StatusID,
                                Price = k.Price,
                                ModifierID = k.ModifierID,
                                Cost = k.Cost,
                                LastUpdateBy = k.LastUpdateBy,
                                LastUpdateDT = k.LastUpdateDT,
                                OrderDetailID = k.OrderDetailID,
                                OrderDetailModifierID = k.OrderDetailModifierID,
                                Quantity = k.Quantity,
                                ModifierName = k.ModifierName
                            });
                        }
                        lstODA = new List<OrderAddonsBLL>();
                        foreach (var ad in _dsorderdetailaddon.Where(x => x.StatusID == 201 && x.OrderDetailID == j.OrderDetailID))
                        {
                            lstODA.Add(new OrderAddonsBLL
                            {
                                StatusID = ad.StatusID,
                                Price = ad.Price,
                                AddonID = ad.AddonID,
                                Cost = ad.Cost,
                                LastUpdateBy = ad.LastUpdateBy,
                                LastUpdateDT = ad.LastUpdateDT,
                                OrderDetailID = ad.OrderDetailID,
                                OrderDetailAddonID = ad.OrderDetailAddonID,
                                Quantity = ad.Quantity,
                                AddonName = ad.AddonName
                            });
                        }
                        lstOD.Add(new OrderDetailBLL
                        {
                            StatusID = j.StatusID,
                            Cost = j.Cost,
                            Price = j.Price,
                            Quantity = j.Quantity == null ? 0 : j.Quantity,
                            OrderDetailID = j.OrderDetailID,
                            LastUpdateDT = j.LastUpdateDT,
                            LastUpdateBy = j.LastUpdateBy,
                            OrderDetailAddons = lstODA,
                            ItemID = j.ItemID,
                            ItemName = j.ItemName,
                            OrderDetailModifiers = lstODM,
                            OrderID = j.OrderID,
                            OrderMode = j.OrderMode
                        });
                    }

                    var _OC = _dsOrdercheckout.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OC != null)
                    {
                        oc = new OrderCheckoutBLL();
                        oc.AmountPaid = _OC.AmountPaid;
                        oc.AmountTotal = _OC.AmountTotal;
                        oc.CheckoutDate = _OC.CheckoutDate.ToString();
                        oc.GrandTotal = _OC.GrandTotal;
                        oc.OrderCheckoutID = _OC.OrderCheckoutID;
                        oc.PaymentMode = _OC.PaymentMode;
                        oc.DiscountAmount = _OC.DiscountAmount;
                        oc.Tax = _OC.Tax;
                        oc.ServiceCharges = _OC.ServiceCharges == null ? 0 : _OC.ServiceCharges;
                        oc.StatusID = _OC.StatusID;
                        oc.LastUpdatedDate = _OC.LastUpdatedDate;
                        oc.LastUpdateBy = _OC.LastUpdateBy;
                        oc.OrderID = _OC.OrderID;
                    }
                    else oc = null;

                    var _OCustomer = _dsOrderCustomerData.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OCustomer != null)
                    {
                        ocustomer = new OrderCustomerBLL();
                        ocustomer.OrderID = _OCustomer.OrderID;
                        ocustomer.Address = _OCustomer.Address;
                        ocustomer.Name = _OCustomer.Name;
                        ocustomer.CustomerOrderID = _OCustomer.CustomerOrderID;
                        ocustomer.Description = _OCustomer.Description;
                        ocustomer.Mobile = _OCustomer.Mobile;
                        ocustomer.Email = _OCustomer.Email;
                        ocustomer.LastUpdatedBy = _OCustomer.LastUpdatedBy;
                        ocustomer.StatusID = _OCustomer.StatusID;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.Latitude = _OCustomer.Latitude;
                        ocustomer.Longitude = _OCustomer.Longitude;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.AddressNickName = _OCustomer.AddressNickName == null ? "" : _OCustomer.AddressNickName;
                        ocustomer.AddressType = _OCustomer.AddressType == null ? "Other" : _OCustomer.AddressType;
                        ocustomer.OrderID = _OCustomer.OrderID;
                    }
                    else ocustomer = null;


                    #region orderstatus
                    lstOrderStatus = new OrderStatusBLL();

                    status1 = new OrderStatusChildBLL();
                    status1 = new OrderStatusChildBLL
                    {
                        Value = "Order Confirmed",
                        Description = "Order has been received",
                        Status = true,
                        Time = Convert.ToDateTime(i.OrderDate).ToShortTimeString()
                    };

                    var chkpreparedate = i.OrderPreparedDate == null ? false : true;
                    var chkpreparedateTime = i.OrderPreparedDate == null ? "" : Convert.ToDateTime(i.OrderPreparedDate).ToShortTimeString();

                    status2 = new OrderStatusChildBLL();
                    status2 = new OrderStatusChildBLL
                    {
                        Value = "Food prepared",
                        Description = "Order has been prepared",
                        Status = chkpreparedate,
                        Time = chkpreparedateTime
                    };

                    var chkOFD = i.OrderOFDDate == null ? false : true;
                    var chkOFDTime = i.OrderOFDDate == null ? "" : Convert.ToDateTime(i.OrderOFDDate).ToShortTimeString();

                    status3 = new OrderStatusChildBLL();
                    status3 = new OrderStatusChildBLL
                    {
                        Value = "Delivery in Progress",
                        Description = "You food is in the way",
                        Status = chkOFD,
                        Time = chkOFDTime
                    };
                    lstOrderStatus.DeliveryinProgress = status3;
                    lstOrderStatus.FoodPrepared = status2;
                    lstOrderStatus.OrderConfirmed = status1;
                    #endregion orderstatus

                    bll.Add(new OrdersBLL
                    {
                        StatusID = i.StatusID,
                        LastUpdateDT = i.LastUpdateDT,
                        LastUpdateBy = i.LastUpdateBy,
                        OrderID = i.OrderID,
                        CustomerID = i.CustomerID,
                        DeliverUserID = i.DeliverUserID,
                        LocationID = i.LocationID,
                        OrderDate = i.OrderDate.ToString(),
                        OrderDeliveryDate = Convert.ToDateTime(i.OrderDate).AddMinutes(45).ToShortTimeString(),
                        OrderNo = i.OrderNo,
                        OrderTakerID = i.OrderTakerID,
                        OrderType = i.OrderType,
                        SessionID = i.SessionID,
                        TransactionNo = i.TransactionNo,
                        BrandName = i.BrandName,
                        BrandLogo = i.BrandLogo == null ? "" : ConfigurationManager.AppSettings["AdminURL"].ToString() + i.BrandLogo,
                        OrderDetails = lstOD,
                        OrderCheckouts = oc,
                        BrandID = i.BrandID,
                        OrderDoneDate = i.OrderDoneDate,
                        OrderOFDDate = i.OrderOFDDate,
                        OrderPreparedDate = i.OrderPreparedDate,
                        Remarks = i.Remarks,
                        OrderStatus = lstOrderStatus,
                        CustomerOrders = ocustomer,
                        //AdvanceOrderDate = i.AdvanceOrderDate,
                        IsAdvanceOrder = i.IsAdvanceOrder,
                        AdvanceOrderPunchDate = i.AdvanceOrderPunchDate,
                    });
                }
                rsp.Orders = bll;
                rsp.status = 1;
                rsp.description = "Success";


                return rsp;
            }
            catch (Exception ex)
            {
                rsp.Orders = bll;
                rsp.status = 0;
                rsp.description = "Failed";
                return rsp;
            }
        }
        public RspOrderPunch OrderPunch(OrdersBLL obj)
        {
            RspOrderPunch rsp;
            try
            {
                if (obj.AppVersion != "26")
                {
                    rsp = new RspOrderPunch();
                    rsp.status = 1006;
                    rsp.description = "Your App Version is not Updated";
                    rsp.OrderID = 0;
                    return rsp;
                }
                var currDate = DateTime.UtcNow.AddMinutes(300);
                var isAllowcheckout = true;
                if (obj.OrderDetails.Count == 0)
                {
                    rsp = new RspOrderPunch();
                    rsp.status = (int)eStatus.Exception;
                    rsp.description = "Sorry, Your order is failed to punched.";
                    rsp.OrderID = 0;
                    return rsp;
                }
                var settings = DBContext.Locations.Where(x => x.LocationID == obj.LocationID).FirstOrDefault();
                if (settings != null)
                {
                    try
                    {
                        if (settings.Opentime != null && settings.Closetime != null)
                        {
                            var t1 = int.Parse(TimeSpan.Parse(settings.Opentime).ToString("hhmm"));
                            var t2 = 2359;
                            var t3 = 0001;
                            var t4 = int.Parse(TimeSpan.Parse(settings.Closetime).ToString("hhmm"));
                            var currTimeint = int.Parse(Convert.ToDateTime(currDate).ToString("HHmm"));
                            isAllowcheckout = (currTimeint > t1 && currTimeint < t2) || (currTimeint > t3 && currTimeint < t4) ? true : false;
                        }
                        if (settings.IsPickupAllowed != 1 && obj.OrderType == "2")
                        {
                            rsp = new RspOrderPunch();
                            rsp.status = (int)eStatus.Exception;
                            rsp.description = "Pickup is temporary closed!";
                            rsp.OrderID = 0;
                            return rsp;
                        }
                        if (settings.IsDeliveryAllowed != 1 && obj.OrderType == "1")
                        {
                            rsp = new RspOrderPunch();
                            rsp.status = (int)eStatus.Exception;
                            rsp.description = "Delivery is temporary closed!";
                            rsp.OrderID = 0;
                            return rsp;
                        }
                        if (settings.IsDineInAllowed != 1 && obj.OrderType == "3")
                        {
                            rsp = new RspOrderPunch();
                            rsp.status = (int)eStatus.Exception;
                            rsp.description = "DineIn is temporary closed!";
                            rsp.OrderID = 0;
                            return rsp;
                        }
                        if (settings.IsAdvanceOrder != 1)
                        {
                            rsp = new RspOrderPunch();
                            rsp.status = (int)eStatus.Exception;
                            rsp.description = "Advance Order is temporary closed!";
                            rsp.OrderID = 0;
                            return rsp;
                        }
                    }
                    catch { }
                }
                if (!isAllowcheckout)
                {
                    rsp = new RspOrderPunch();
                    rsp.status = (int)eStatus.Exception;
                    rsp.description = "Sorry, Delivery Time is from " + Convert.ToDateTime(settings.Opentime).ToString("hh:mm tt") + " to " + Convert.ToDateTime(settings.Closetime).ToString("hh:mm tt");
                    rsp.OrderID = 0;
                    return rsp;
                }
                using (var dbContextTransaction = DBContext.Database.BeginTransaction())
                {
                    try
                    {
                        //var currDate = DateTime.UtcNow.AddMinutes(300);
                        var tempCustomerOrders = obj.CustomerOrders;
                        var tempCheckout = obj.OrderCheckouts;
                        obj.CustomerOrders = null;
                        obj.OrderCheckouts = null;

                        Order orders = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(obj)).ToObject<Order>();

                        if (tempCustomerOrders != null)
                        {
                            orders.CustomerOrders = new List<CustomerOrder>();
                            orders.CustomerOrders.Add(new CustomerOrder
                            {
                                Address = tempCustomerOrders.Address,
                                Description = tempCustomerOrders.Description,
                                Email = tempCustomerOrders.Email,
                                Latitude = tempCustomerOrders.Latitude,
                                Longitude = tempCustomerOrders.Longitude,
                                Mobile = tempCustomerOrders.Mobile,
                                Name = tempCustomerOrders.Name,
                                AddressNickName = tempCustomerOrders.AddressNickName,
                                AddressType = tempCustomerOrders.AddressType
                            });
                        }

                        if (tempCheckout != null)
                        {
                            orders.OrderCheckouts = new List<OrderCheckout>();
                            orders.OrderCheckouts.Add(new OrderCheckout
                            {
                                AmountPaid = tempCheckout.AmountPaid,
                                AmountTotal = tempCheckout.AmountTotal,
                                CheckoutDate = DateTime.UtcNow.AddMinutes(300),
                                GrandTotal = tempCheckout.GrandTotal,
                                ServiceCharges = tempCheckout.ServiceCharges == null ? 0 : tempCheckout.ServiceCharges,
                                PaymentMode = tempCheckout.PaymentMode,
                                Tax = tempCheckout.Tax,
                                DiscountAmount = tempCheckout.DiscountAmount ?? 0,
                                StatusID = 101
                            });
                        }
                        orders.TransactionNo = DBContext.Orders.Where(x => x.Location.BrandID == obj.BrandID).Max(x => x.TransactionNo);

                        var ondate = obj.IsAdvanceOrder == true ? Convert.ToDateTime(obj.AdvanceOrderDate).Date : currDate.Date;

                        orders.OrderNo = DBContext.Orders.Where(x =>
                        x.LocationID == orders.LocationID
                        && DbFunctions.TruncateTime(x.OrderDate) == currDate.Date
                        ).Max(x => x.OrderNo);
                        orders.TransactionNo = orders.TransactionNo == null ? 1 : orders.TransactionNo + 1;
                        orders.OrderNo = orders.OrderNo == null ? 1 : orders.OrderNo + 1;
                        if (orders.IsAdvanceOrder == true)
                        {
                            orders.OrderDate = obj.AdvanceOrderDate;
                        }
                        else
                        {
                            orders.OrderDate = DateTime.UtcNow.AddMinutes(300);

                        }
                        orders.OrderDate = DateTime.UtcNow.AddMinutes(300);
                        orders.LastUpdateBy = orders.CustomerID.ToString();
                        orders.LastUpdateDT = DateTime.UtcNow.AddMinutes(300);
                        orders.OrderType = obj.OrderType;
                        orders.StatusID = 101;

                        orders.AdvanceOrderPunchDate = DateTime.UtcNow.AddMinutes(300);

                        foreach (var i in orders.OrderDetails)
                        {
                            i.Price = Math.Round((Convert.ToDouble(i.Quantity) * Convert.ToDouble(i.Price)), 2);
                            i.OrderMode = "New";
                            i.StatusID = 201;
                            i.LastUpdateBy = orders.CustomerID.ToString();
                            i.LastUpdateDT = DateTime.UtcNow.AddMinutes(300);
                            if (i.OrderDetailModifiers != null)
                            {
                                foreach (var j in i.OrderDetailModifiers)
                                {
                                    j.StatusID = 201;
                                    j.LastUpdateBy = orders.CustomerID.ToString();
                                    j.LastUpdateDT = DateTime.UtcNow.AddMinutes(300);
                                }
                            }
                            if (i.OrderDetailAddons != null)
                            {
                                foreach (var j in i.OrderDetailAddons)
                                {
                                    j.StatusID = 201;
                                    j.LastUpdateBy = orders.CustomerID.ToString();
                                    j.LastUpdateDT = DateTime.UtcNow.AddMinutes(300);
                                }
                            }
                        }
                        foreach (var item in orders.CustomerOrders)
                        {
                            item.StatusID = 1;
                            item.LastUpdatedBy = orders.CustomerID.ToString();
                            item.LastUpdatedDate = DateTime.UtcNow.AddMinutes(300);
                        }
                        Order data = DBContext.Orders.Add(orders);
                        DBContext.SaveChanges();
                        dbContextTransaction.Commit();
                        try
                        {
                            var getTokens = DBContext.PushTokens.Where(x => x.LocationID == obj.LocationID).ToList();
                            foreach (var item in getTokens)
                            {
                                var token = new PushNoticationBLL();
                                obj.BrandName = obj.BrandName ?? "";
                                token.Title = obj.BrandName == "" ? "Z-Six" : obj.BrandName + " | New Order for Delivery";
                                token.Message = "You have new order for delivery.";
                                token.DeviceID = item.Token;
                                PushNotificationAndroid(token);
                            }
                        }
                        catch (Exception)
                        {
                        }
                        rsp = new RspOrderPunch();
                        rsp.status = (int)eStatus.Success;
                        rsp.description = "Your order has been punched successfully.";
                        rsp.OrderID = data.OrderID;
                        rsp.OrderNo = data.OrderNo;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        rsp = new RspOrderPunch();
                        rsp.status = (int)eStatus.Exception;
                        rsp.description = "Sorry, Your order is failed to punched.";
                        rsp.OrderID = 0;
                        rsp.OrderNo = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                rsp = new RspOrderPunch();
                rsp.status = (int)eStatus.Exception;
                rsp.description = "Sorry, Your order is failed to punched.";
                rsp.OrderID = 0;
                rsp.OrderNo = 0;
            }
            return rsp;
        }
        public RspOrdersAdmin GetOrdersAdmin(int locationID)
        {
            var bll = new List<OrdersBLL>();
            var lstOD = new List<OrderDetailBLL>();
            var lstODM = new List<OrderModifiersBLL>();
            var lstODA = new List<OrderAddonsBLL>();
            var oc = new OrderCheckoutBLL();
            var ocustomer = new OrderCustomerBLL();
            var rsp = new RspOrdersAdmin();
            try
            {
                var list = DBContext.Orders.Where(x => x.LocationID == locationID).ToList();

                foreach (var i in list)
                {
                    lstOD = new List<OrderDetailBLL>();
                    foreach (var j in i.OrderDetails.Where(x => x.StatusID == 1))
                    {
                        lstODM = new List<OrderModifiersBLL>();
                        foreach (var k in j.OrderDetailModifiers.Where(x => x.StatusID == 1))
                        {
                            lstODM.Add(new OrderModifiersBLL
                            {
                                StatusID = k.StatusID,
                                Price = k.Price,
                                ModifierID = k.ModifierID,
                                Cost = k.Cost,
                                LastUpdateBy = k.LastUpdateBy,
                                LastUpdateDT = k.LastUpdateDT,
                                OrderDetailID = k.OrderDetailID,
                                OrderDetailModifierID = k.OrderDetailModifierID,
                                Quantity = k.Quantity
                            });
                        }
                        lstODA = new List<OrderAddonsBLL>();
                        foreach (var k in j.OrderDetailAddons.Where(x => x.StatusID == 1))
                        {
                            lstODA.Add(new OrderAddonsBLL
                            {
                                StatusID = k.StatusID,
                                Price = k.Price,
                                AddonID = k.AddonID,
                                Cost = k.Cost,
                                LastUpdateBy = k.LastUpdateBy,
                                LastUpdateDT = k.LastUpdateDT,
                                OrderDetailID = k.OrderDetailID,
                                OrderDetailAddonID = k.OrderDetailAddonID,
                                Quantity = k.Quantity
                            });
                        }
                        lstOD.Add(new OrderDetailBLL
                        {
                            StatusID = j.StatusID,
                            Cost = j.Cost,
                            Price = j.Price,
                            Quantity = j.Quantity,
                            OrderDetailID = j.OrderDetailID,
                            LastUpdateDT = j.LastUpdateDT,
                            LastUpdateBy = j.LastUpdateBy,
                            OrderDetailAddons = lstODA,
                            ItemID = j.ItemID,
                            OrderDetailModifiers = lstODM,
                            OrderID = j.OrderID,
                            OrderMode = j.OrderMode
                        });
                    }

                    var _OC = i.OrderCheckouts.FirstOrDefault();
                    if (_OC != null)
                    {
                        oc.AmountPaid = _OC.AmountPaid;
                        oc.AmountTotal = _OC.AmountTotal;
                        oc.CheckoutDate = Convert.ToDateTime(_OC.CheckoutDate).ToString("yyyy-MM-dd HH:mm:ss");
                        oc.GrandTotal = _OC.GrandTotal;
                        oc.OrderCheckoutID = _OC.OrderCheckoutID;
                        oc.PaymentMode = _OC.PaymentMode;
                        oc.Tax = _OC.Tax;
                        oc.StatusID = _OC.StatusID;
                        oc.LastUpdatedDate = _OC.LastUpdatedDate;
                        oc.LastUpdateBy = _OC.LastUpdateBy;
                        oc.OrderID = _OC.OrderID;
                    }
                    else oc = null;

                    var _OCustomer = i.CustomerOrders.FirstOrDefault();
                    if (_OCustomer != null)
                    {
                        ocustomer.OrderID = _OCustomer.OrderID;
                        ocustomer.Address = _OCustomer.Address;
                        ocustomer.CustomerOrderID = _OCustomer.CustomerOrderID;
                        ocustomer.Description = _OCustomer.Description;
                        ocustomer.Mobile = _OCustomer.Mobile;
                        ocustomer.Email = _OCustomer.Email;
                        ocustomer.LastUpdatedBy = _OCustomer.LastUpdatedBy;
                        ocustomer.StatusID = _OCustomer.StatusID;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.Latitude = _OCustomer.Latitude;
                        ocustomer.Longitude = _OCustomer.Longitude;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;

                        ocustomer.OrderID = _OCustomer.OrderID;
                    }
                    else ocustomer = null;


                    bll.Add(new OrdersBLL
                    {
                        StatusID = i.StatusID,
                        LastUpdateDT = i.LastUpdateDT,
                        LastUpdateBy = i.LastUpdateBy,
                        OrderID = i.OrderID,
                        CustomerID = i.CustomerID,
                        DeliverUserID = i.DeliverUserID,
                        LocationID = i.LocationID,
                        OrderDate = Convert.ToDateTime(i.OrderDate).ToString("dd-MM-yyyy HH:mm:ss"),
                        OrderNo = i.OrderNo,
                        OrderTakerID = i.OrderTakerID,
                        OrderType = i.OrderType,
                        SessionID = i.SessionID,
                        TransactionNo = i.TransactionNo,
                        AdvanceOrderPunchDate = DateTime.UtcNow,
                        //AdvanceOrderDate = i.AdvanceOrderDate,
                        IsAdvanceOrder = i.IsAdvanceOrder,

                        OrderDetails = lstOD,
                        OrderCheckouts = oc,
                        //CustomerOrders = ocustomer
                    });
                }
                rsp.Orders = bll;
                rsp.status = 1;
                rsp.description = "Success";


                return rsp;
            }
            catch (Exception ex)
            {
                rsp.Orders = bll;
                rsp.status = 0;
                rsp.description = "Failed";
                return rsp;
            }
        }
        public RspOrdersCustomer GetOrdersAdminV2(int LocationID, string StartDate, string EndDate, string Search)
        {
            var status1 = new OrderStatusChildBLL();
            var status2 = new OrderStatusChildBLL();
            var status3 = new OrderStatusChildBLL();
            var bll = new List<OrdersBLL>();
            var lstOD = new List<OrderDetailBLL>();
            var lstODM = new List<OrderModifiersBLL>();
            var oc = new OrderCheckoutBLL();
            var ocustomer = new OrderCustomerBLL();
            var lstOrderStatus = new OrderStatusBLL();
            var rsp = new RspOrdersCustomer();
            try
            {
                var ds = GetAdminOrder_ADOV2(LocationID, StartDate, EndDate, Search);
                var _dsOrders = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0])).ToObject<List<OrdersBLL>>();
                var _dsorderdetail = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1])).ToObject<List<OrderDetailBLL>>();
                var _dsorderdetailmodifier = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[2])).ToObject<List<OrderModifiersBLL>>();
                var _dsOrdercheckout = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[3])).ToObject<List<OrderCheckoutBLL>>();
                var _dsOrderCustomerData = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[4])).ToObject<List<OrderCustomerBLL>>();
                //var _dsLocation = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject<LocationsBLL>();
                //var _dsBrand = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject< List<BrandsBLL>>().FirstOrDefault();

                //var list = new List<Order>();
                //list = orderID == 0 ?
                //    DBContext.Orders.Where(x => x.CustomerID == customerID).ToList()
                //    : DBContext.Orders.Where(x => x.CustomerID == customerID && x.OrderID == orderID).ToList();

                foreach (var i in _dsOrders.OrderByDescending(x => x.OrderID))
                {
                    lstOD = new List<OrderDetailBLL>();
                    foreach (var j in _dsorderdetail.Where(x => x.StatusID == 201 && x.OrderID == i.OrderID))
                    {
                        lstODM = new List<OrderModifiersBLL>();
                        foreach (var k in _dsorderdetailmodifier.Where(x => x.StatusID == 201 && x.OrderDetailID == j.OrderDetailID))
                        {
                            lstODM.Add(new OrderModifiersBLL
                            {
                                StatusID = k.StatusID,
                                Price = k.Price,
                                ModifierID = k.ModifierID,
                                Cost = k.Cost,
                                LastUpdateBy = k.LastUpdateBy,
                                LastUpdateDT = k.LastUpdateDT,
                                OrderDetailID = k.OrderDetailID,
                                OrderDetailModifierID = k.OrderDetailModifierID,
                                Quantity = k.Quantity,
                                ModifierName = k.ModifierName
                            });
                        }

                        lstOD.Add(new OrderDetailBLL
                        {
                            StatusID = j.StatusID,
                            Cost = j.Cost,
                            Price = j.Price,
                            Quantity = j.Quantity == null ? 0 : j.Quantity,
                            OrderDetailID = j.OrderDetailID,
                            LastUpdateDT = j.LastUpdateDT,
                            LastUpdateBy = j.LastUpdateBy,
                            ItemID = j.ItemID,
                            ItemName = j.ItemName,
                            OrderDetailModifiers = lstODM,
                            OrderID = j.OrderID,
                            OrderMode = j.OrderMode
                        });
                    }

                    var _OC = _dsOrdercheckout.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OC != null)
                    {
                        oc = new OrderCheckoutBLL();
                        oc.AmountPaid = _OC.AmountPaid;
                        oc.AmountTotal = _OC.AmountTotal;
                        oc.CheckoutDate = Convert.ToDateTime(_OC.CheckoutDate).ToString("yyyy-MM-dd HH:mm:ss");
                        oc.GrandTotal = _OC.GrandTotal;
                        oc.OrderCheckoutID = _OC.OrderCheckoutID;
                        oc.PaymentMode = _OC.PaymentMode;
                        oc.Tax = _OC.Tax;
                        oc.DiscountAmount = _OC.DiscountAmount ?? 0;
                        oc.ServiceCharges = _OC.ServiceCharges == null ? 0 : _OC.ServiceCharges;
                        oc.StatusID = _OC.StatusID;
                        oc.LastUpdatedDate = _OC.LastUpdatedDate;
                        oc.LastUpdateBy = _OC.LastUpdateBy;
                        oc.OrderID = _OC.OrderID;
                    }
                    else oc = null;

                    var _OCustomer = _dsOrderCustomerData.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OCustomer != null)
                    {
                        ocustomer = new OrderCustomerBLL();
                        ocustomer.OrderID = _OCustomer.OrderID;
                        ocustomer.Address = _OCustomer.Address;
                        ocustomer.Name = _OCustomer.Name;
                        ocustomer.CustomerOrderID = _OCustomer.CustomerOrderID;
                        ocustomer.Description = _OCustomer.Description;
                        ocustomer.Mobile = _OCustomer.Mobile;
                        ocustomer.Email = _OCustomer.Email;
                        ocustomer.LastUpdatedBy = _OCustomer.LastUpdatedBy;
                        ocustomer.StatusID = _OCustomer.StatusID;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.Latitude = _OCustomer.Latitude;
                        ocustomer.Longitude = _OCustomer.Longitude;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.AddressNickName = _OCustomer.AddressNickName == null ? "" : _OCustomer.AddressNickName;
                        ocustomer.AddressType = _OCustomer.AddressType == null ? "Other" : _OCustomer.AddressType;
                        ocustomer.OrderID = _OCustomer.OrderID;
                    }
                    else ocustomer = null;


                    bll.Add(new OrdersBLL
                    {
                        StatusID = i.StatusID,
                        LastUpdateDT = i.LastUpdateDT,
                        LastUpdateBy = i.LastUpdateBy,
                        OrderID = i.OrderID,
                        CustomerID = i.CustomerID,
                        DeliverUserID = i.DeliverUserID,
                        LocationID = i.LocationID,
                        OrderDate = Convert.ToDateTime(i.OrderDate).ToString("dd/MM/yyyy hh:mm tt"),
                        OrderNo = i.OrderNo,
                        OrderTakerID = i.OrderTakerID,
                        OrderType = i.OrderType,
                        SessionID = i.SessionID,
                        TransactionNo = i.TransactionNo,
                        BrandName = i.BrandName,
                        BrandLogo = i.BrandLogo == null ? "" : ConfigurationManager.AppSettings["AdminURL"].ToString() + i.BrandLogo,
                        OrderDetails = lstOD,
                        OrderCheckouts = oc,
                        CustomerOrders = ocustomer,
                        BrandID = i.BrandID,
                        OrderDoneDate = i.OrderDoneDate,
                        OrderOFDDate = i.OrderOFDDate,
                        OrderPreparedDate = i.OrderPreparedDate,
                        Remarks = i.Remarks,
                        OrderStatus = i.OrderStatus,
                        AdvanceOrderPunchDate = i.AdvanceOrderPunchDate,
                        //AdvanceOrderDate = i.AdvanceOrderDate,
                        IsAdvanceOrder = i.IsAdvanceOrder,
                    });
                }
                rsp.Orders = bll;
                rsp.status = 1;
                rsp.description = "Success";


                return rsp;
            }
            catch (Exception ex)
            {
                rsp.Orders = bll;
                rsp.status = 0;
                rsp.description = "Failed";
                return rsp;
            }
        }
        public RspOrdersCustomer GetOrdersAdminV2(int LocationID)
        {
            var status1 = new OrderStatusChildBLL();
            var status2 = new OrderStatusChildBLL();
            var status3 = new OrderStatusChildBLL();
            var bll = new List<OrdersBLL>();
            var lstOD = new List<OrderDetailBLL>();
            var lstODM = new List<OrderModifiersBLL>();
            var oc = new OrderCheckoutBLL();
            var ocustomer = new OrderCustomerBLL();
            var lstOrderStatus = new OrderStatusBLL();
            var rsp = new RspOrdersCustomer();
            try
            {
                var ds = GetAdminOrder_ADO(LocationID);
                var _dsOrders = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0])).ToObject<List<OrdersBLL>>();
                var _dsorderdetail = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[1])).ToObject<List<OrderDetailBLL>>();
                var _dsorderdetailmodifier = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[2])).ToObject<List<OrderModifiersBLL>>();
                var _dsOrdercheckout = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[3])).ToObject<List<OrderCheckoutBLL>>();
                var _dsOrderCustomerData = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[4])).ToObject<List<OrderCustomerBLL>>();
                //var _dsLocation = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject<LocationsBLL>();
                //var _dsBrand = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[5])).ToObject< List<BrandsBLL>>().FirstOrDefault();

                //var list = new List<Order>();
                //list = orderID == 0 ?
                //    DBContext.Orders.Where(x => x.CustomerID == customerID).ToList()
                //    : DBContext.Orders.Where(x => x.CustomerID == customerID && x.OrderID == orderID).ToList();

                foreach (var i in _dsOrders.OrderByDescending(x => x.OrderID))
                {
                    lstOD = new List<OrderDetailBLL>();
                    foreach (var j in _dsorderdetail.Where(x => x.StatusID == 201 && x.OrderID == i.OrderID))
                    {
                        lstODM = new List<OrderModifiersBLL>();
                        foreach (var k in _dsorderdetailmodifier.Where(x => x.StatusID == 201 && x.OrderDetailID == j.OrderDetailID))
                        {
                            lstODM.Add(new OrderModifiersBLL
                            {
                                StatusID = k.StatusID,
                                Price = k.Price,
                                ModifierID = k.ModifierID,
                                Cost = k.Cost,
                                LastUpdateBy = k.LastUpdateBy,
                                LastUpdateDT = k.LastUpdateDT,
                                OrderDetailID = k.OrderDetailID,
                                OrderDetailModifierID = k.OrderDetailModifierID,
                                Quantity = k.Quantity,
                                ModifierName = k.ModifierName
                            });
                        }

                        lstOD.Add(new OrderDetailBLL
                        {
                            StatusID = j.StatusID,
                            Cost = j.Cost,
                            Price = j.Price,
                            Quantity = j.Quantity == null ? 0 : j.Quantity,
                            OrderDetailID = j.OrderDetailID,
                            LastUpdateDT = j.LastUpdateDT,
                            LastUpdateBy = j.LastUpdateBy,
                            ItemID = j.ItemID,
                            ItemName = j.ItemName,
                            OrderDetailModifiers = lstODM,
                            OrderID = j.OrderID,
                            OrderMode = j.OrderMode
                        });
                    }

                    var _OC = _dsOrdercheckout.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OC != null)
                    {
                        oc = new OrderCheckoutBLL();
                        oc.AmountPaid = _OC.AmountPaid;
                        oc.AmountTotal = _OC.AmountTotal;
                        oc.CheckoutDate = Convert.ToDateTime(_OC.CheckoutDate).ToString("yyyy-MM-dd HH:mm:ss");
                        oc.GrandTotal = _OC.GrandTotal;
                        oc.OrderCheckoutID = _OC.OrderCheckoutID;
                        oc.PaymentMode = _OC.PaymentMode;
                        oc.DiscountAmount = _OC.DiscountAmount;
                        oc.Tax = _OC.Tax;
                        oc.ServiceCharges = _OC.ServiceCharges == null ? 0 : _OC.ServiceCharges;
                        oc.StatusID = _OC.StatusID;
                        oc.LastUpdatedDate = _OC.LastUpdatedDate;
                        oc.LastUpdateBy = _OC.LastUpdateBy;
                        oc.OrderID = _OC.OrderID;
                    }
                    else oc = null;

                    var _OCustomer = _dsOrderCustomerData.Where(x => x.OrderID == i.OrderID).FirstOrDefault();
                    if (_OCustomer != null)
                    {
                        ocustomer = new OrderCustomerBLL();
                        ocustomer.OrderID = _OCustomer.OrderID;
                        ocustomer.Address = _OCustomer.Address;
                        ocustomer.Name = _OCustomer.Name;
                        ocustomer.CustomerOrderID = _OCustomer.CustomerOrderID;
                        ocustomer.Description = _OCustomer.Description;
                        ocustomer.Mobile = _OCustomer.Mobile;
                        ocustomer.Email = _OCustomer.Email;
                        ocustomer.LastUpdatedBy = _OCustomer.LastUpdatedBy;
                        ocustomer.StatusID = _OCustomer.StatusID;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.Latitude = _OCustomer.Latitude;
                        ocustomer.Longitude = _OCustomer.Longitude;
                        ocustomer.LastUpdatedDate = _OCustomer.LastUpdatedDate;
                        ocustomer.AddressNickName = _OCustomer.AddressNickName == null ? "" : _OCustomer.AddressNickName;
                        ocustomer.AddressType = _OCustomer.AddressType == null ? "Other" : _OCustomer.AddressType;
                        ocustomer.OrderID = _OCustomer.OrderID;
                    }
                    else ocustomer = null;


                    bll.Add(new OrdersBLL
                    {
                        StatusID = i.StatusID,
                        LastUpdateDT = i.LastUpdateDT,
                        LastUpdateBy = i.LastUpdateBy,
                        OrderID = i.OrderID,
                        CustomerID = i.CustomerID,
                        DeliverUserID = i.DeliverUserID,
                        LocationID = i.LocationID,
                        OrderDate = Convert.ToDateTime(i.OrderDate).ToString("dd/MM/yyyy hh:mm tt"),
                        OrderNo = i.OrderNo,
                        OrderTakerID = i.OrderTakerID,
                        OrderType = i.OrderType,
                        SessionID = i.SessionID,
                        TransactionNo = i.TransactionNo,
                        BrandName = i.BrandName,
                        BrandLogo = i.BrandLogo == null ? "" : ConfigurationManager.AppSettings["AdminURL"].ToString() + i.BrandLogo,
                        OrderDetails = lstOD,
                        OrderCheckouts = oc,
                        CustomerOrders = ocustomer,
                        //AdvanceOrderDate=i.AdvanceOrderDate,
                        AdvanceOrderPunchDate = DateTime.UtcNow,
                        IsAdvanceOrder = i.IsAdvanceOrder,
                        BrandID = i.BrandID,
                        OrderDoneDate = i.OrderDoneDate,
                        OrderOFDDate = i.OrderOFDDate,
                        OrderPreparedDate = i.OrderPreparedDate,
                        Remarks = i.Remarks,
                        OrderStatus = i.OrderStatus
                    });
                }
                rsp.Orders = bll;
                rsp.status = 1;
                rsp.description = "Success";


                return rsp;
            }
            catch (Exception ex)
            {
                rsp.Orders = bll;
                rsp.status = 0;
                rsp.description = "Failed";
                return rsp;
            }
        }
        public Rsp CancelOrder(OrdersBLL obj)
        {
            Rsp rsp = new Rsp();

            using (var dbContextTransaction = DBContext.Database.BeginTransaction())
            {
                try
                {
                    if (obj.OrderID == 0 || obj.OrderID == null)
                    {
                        rsp.status = (int)eStatus.Exception;
                        rsp.description = "Order cannot be cancel due to invalid parameter";
                    }
                    else
                    {
                        var currDate = DateTime.UtcNow.AddMinutes(300);

                        Order order = DBContext.Orders.Where(x => x.OrderID == obj.OrderID).FirstOrDefault();
                        order.StatusID = 104;
                        order.LastUpdateDT = currDate;
                        DBContext.Orders.AddOrUpdate(order);
                        DBContext.SaveChanges();
                        dbContextTransaction.Commit();

                        rsp.status = (int)eStatus.Success;
                        rsp.description = "Your Orders has been cancelled succesfully";
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    rsp.status = (int)eStatus.Exception;
                    rsp.description = "Sorry! Order cannot be cancelled.";

                }
            }

            return rsp;
        }
        public Rsp UpdateOrderAdmin(int OrderID, int StatusID)
        {
            Rsp rsp = new Rsp();

            using (var dbContextTransaction = DBContext.Database.BeginTransaction())
            {
                try
                {
                    if (OrderID == 0 || StatusID == 0)
                    {
                        rsp.status = (int)eStatus.Exception;
                        rsp.description = "Order cannot be update due to invalid parameter";
                    }
                    else
                    {
                        var currDate = DateTime.UtcNow.AddMinutes(300);

                        Order order = DBContext.Orders.Where(x => x.OrderID == OrderID).FirstOrDefault();

                        if (StatusID == 102)
                        {
                            order.OrderPreparedDate = currDate;
                        }
                        if (StatusID == 103)
                        {
                            order.OrderOFDDate = currDate;
                        }

                        order.StatusID = StatusID;
                        order.LastUpdateDT = currDate;
                        DBContext.Orders.AddOrUpdate(order);
                        DBContext.SaveChanges();
                        dbContextTransaction.Commit();

                        rsp.status = (int)eStatus.Success;
                        rsp.description = "Order has been updated succesfully";
                    }
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    rsp.status = (int)eStatus.Exception;
                    rsp.description = "Sorry! Order cannot be updated.";
                }
            }

            return rsp;
        }
        public Rsp TransferOrder(int OrderID, int FromLocationID, int ToLocationID)
        {
            Rsp rsp = new Rsp();

            using (var dbContextTransaction = DBContext.Database.BeginTransaction())
            {
                try
                {
                    var location = DBContext.Locations.Where(x => x.BrandID == ToLocationID).FirstOrDefault();
                    var torder = new TransferOrder();
                    torder.FromLocationID = FromLocationID;
                    torder.ToLocationID = location.LocationID;
                    torder.OrderID = OrderID;

                    DBContext.TransferOrders.AddOrUpdate(torder);
                    DBContext.SaveChanges();
                    dbContextTransaction.Commit();

                    try
                    {
                        var getTokens = DBContext.PushTokens.Where(x => x.LocationID == torder.ToLocationID).ToList();
                        foreach (var item in getTokens)
                        {
                            var token = new PushNoticationBLL();
                            token.Title = location.Brand.Name + " | New Order";
                            token.Message = "You have new order for delivery.";
                            token.DeviceID = item.Token;
                            PushNotificationAndroid(token);
                        }
                    }
                    catch (Exception)
                    {
                    }

                    rsp.status = (int)eStatus.Success;
                    rsp.description = "Order has been updated succesfully";
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    rsp.status = (int)eStatus.Exception;
                    rsp.description = "Sorry! Order cannot be updated.";
                }
            }

            return rsp;
        }
        public DataSet GetCustomerOrder_ADO(int CustomerID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@CustomerID", CustomerID);
                //p[1] = new SqlParameter("@LocationID", LocationID);
                //p[2] = new SqlParameter("@BrandID", BrandID);
                ds = (new DBHelper().GetDatasetFromSP)("sp_GetCustomerOrders_api", p);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataSet GetAdminOrder_ADOV2(int LocationID, string StartDate, string EndDate, string Search)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] p = new SqlParameter[4];
                p[0] = new SqlParameter("@LocationID", LocationID);
                p[1] = new SqlParameter("@StartDate", StartDate);
                p[2] = new SqlParameter("@EndDate", EndDate);
                p[3] = new SqlParameter("@Search", Search);

                ds = (new DBHelper().GetDatasetFromSP)("sp_GetAdminOrdersv2_api", p);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataSet GetAdminOrder_ADO(int LocationID)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlParameter[] p = new SqlParameter[1];
                p[0] = new SqlParameter("@LocationID", LocationID);

                ds = (new DBHelper().GetDatasetFromSP)("sp_GetAdminOrders_api", p);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
