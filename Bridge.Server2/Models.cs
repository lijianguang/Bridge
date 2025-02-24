using B.M;
using LSS.Infrastructure.Storage;
using LSS.VehicleIntegrationTransaction.Model.Commons;
using LSS.VehicleIntegrationTransaction.Model.EntityModel;
using LSS.VehicleIntegrationTransaction.Model.Enums;
using LSS.VehicleIntegrationTransaction.Model.Models;
using LSS.VehicleIntegrationTransaction.Model.Values;
using LSS.VehicleIntegrationTransaction.SalesOrder.Model.EntityModel;
using LSS.VehicleIntegrationTransaction.SalesOrder.Model.EntityModel.SalesOrder;
using LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage1;
using LSS.VehicleSalesOrder.Model.Entities;
using NVS.Storage;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Bridge.Server2.Models
{
    public struct strct
    {
        public int id;
        private int idp;
        public string name;
        public string description;
    }
    public class MsgTmp
    {
        public strct strctf;
        public strct strct { get; set; }
        public string? Name {  get; set; }
        public int Age {  get; set; }
        public MsgTmp1 MsgTmp1 { get; set; }
    }
    public class MsgTmp1
    {
        public MsgTmp2 MsgTmp2 { get; set; }

        public TEST<int,int> T1 {get;set; }
        public TEST<InventoryStatus,int> T2 { get; set; }
    }
}
namespace B.M
{
    public class MsgTmp2
    {
        public int Age { get; set; }
    }
}

namespace LSS.VehicleIntegrationTransaction.Model.Enums
{
    public class Status : Enumeration
    {
        public static Status Intake = new Status(10, nameof(Intake).ToLowerInvariant());
        public static Status Open = new Status(20, nameof(Open).ToLowerInvariant());
        public static Status CBO = new Status(30, nameof(CBO).ToLowerInvariant());
        public static Status Close = new Status(40, nameof(Close).ToLowerInvariant());

        public Status(int id, string name) : base(id, name) { }

        public List<Stage> Stages { get; set; }
    }
}
public class TEST<T1,T2>
{
    public T1 t { get; set; }
    public T2 t2 { get; set; }
}
namespace LSS.VehicleIntegrationTransaction.Model.Enums
{
    public class Stage : Enumeration
    {
        public static Stage Unallocated = new Stage(10, nameof(Unallocated).ToLowerInvariant());
        public static Stage PreAllocated = new Stage(20, nameof(PreAllocated).ToLowerInvariant());
        public static Stage Allocated = new Stage(30, nameof(Allocated).ToLowerInvariant());
        public static Stage Invoiced = new Stage(40, nameof(Invoiced).ToLowerInvariant());
        public static Stage Delivered = new Stage(50, nameof(Delivered).ToLowerInvariant());
        public static Stage Cancelled = new Stage(60, nameof(Cancelled).ToLowerInvariant());

        public Stage(int id, string name) : base(id, name)
        {
        }

        public static IEnumerable<Stage> List() => new[] { Unallocated, PreAllocated, Allocated, Invoiced, Delivered, Cancelled };
    }
}
namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage1
{
    public class SalesOrderDeliveryAddress
    {
        public long Id { get; set; }
        public long SalesOrderHeaderId { get; set; }
        public string DeliverTo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string LocationCode { get; set; }
        public string Payer { get; set; }
        public int Sequence { get; set; }
        public bool? IsDestinationAddress { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Models
{
    public class AddressType : Enumeration
    {
        public static readonly AddressType MainAddress = new AddressType(1, nameof(MainAddress));
        public static readonly AddressType VisitAddress = new AddressType(2, nameof(VisitAddress));
        public AddressType(int id, string name) : base(id, name)
        {
        }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Models
{
    public class Address
    {
        public string AddressDetail { get; set; }
        public string AddressDetailLocal { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CityLocal { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int? AddressTypeId { get; set; }
        public AddressType AddressType { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Models
{
    public class SalesOrderCustomer
    {
        public long Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CentralCustomerId { get; set; }
        public string Prefix { get; set; }
        public string TelephoneNumber { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string VATNumber { get; set; }
        public string CompanyRegNumber { get; set; }
        public string CountryCode { get; set; }
        public string ParmaId { get; set; }
        public Address MailingAddress { get; set; }
        public Address ShappingAddress { get; set; }
        public string PaymentTerm { get; set; }
        public long? PaymentTermId { get; set; }
        public int? PaymentTypeId { get; set; }
        public List<SalesOrderCustomerContact> SalesOrderCustomerContacts { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Models
{
    public class SalesOrderCustomerContact
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public long SalesOrderCustomerId { get; set; }
    }
}
public abstract class Entity : Entity<long>
{
}
public class PaymentCode : Enumeration
{
    public static PaymentCode AfterMarket = new PaymentCode(1, "After Market");
    public static PaymentCode Customer = new PaymentCode(2, "Customer");
    public static PaymentCode MarketCompany = new PaymentCode(3, "Market Company");

    public PaymentCode(int id, string name) : base(id, name)
    {
    }
}
namespace LSS.Infrastructure.Storage
{
    public interface IRowVersion
    {
        [Timestamp]
        byte[] RowVersion { get; set; }
    }
}
public class LineType : PreparationType
{
    public static readonly LineType Vehicle = new LineType(1, nameof(Vehicle));

    public LineType(int id, string name) : base(id, name)
    {
    }
}
public class PreparationType : Enumeration
{
    public static readonly LineType CAWorkShop = new LineType(2, "CAWorkshop");
    public static readonly LineType ExternalWorkShop = new LineType(3, "ExternalWorkshop");
    public static readonly LineType InternalWorkShop = new LineType(4, "InternalWorkshop");
    public static readonly LineType AdditionalService = new LineType(5, "AdditionalService");

    public PreparationType(int id, string name) : base(id, name)
    {
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Enums
{
    public enum InventoryStatus
    {
        Allocated = 3,
        Available = 4,
        Invoiced = 8,
        Delivered = 9,
        Retailed = 10
    }
}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLineVehicle
    {
        public long Id { get; set; }
        public long VehicleId { get; set; }
        public string ChassisSeries { get; set; }
        public string ChassisNumber { get; set; }
        public string Vin { get; set; }
        public string Alias { get; set; }
        public string EngineNumber { get; set; }
        public DateTime? GRDate { get; set; }
        public string Location { get; set; }
        public string AssignToCode { get; set; }
        public string AssignToName { get; set; }
        public long SalesOrderLineId { get; set; }

        public string RegistrationNo { get; set; }
        public DateTime? RegisterDate { get; set; }
        public string CARequestId { get; set; }
        public string CASystemId { get; set; }
        public string CAUnitId { get; set; }
        public int? ImportModeId { get; set; }
        public string ImportModeName { get; set; }
        public string Color { get; set; }
        public decimal? Weight { get; set; }
        public string ExternalRef { get; set; }
        public InventoryStatus? InventoryStatus { get; set; }
        public DateTimeOffset? RetailDate { get; set; }
        public string ExternalRef2 { get; set; }
        public DateTime? ProductionEndDate { get; set; }
        public string CommercialModelCode { get; set; }
        public int? InventoryHorsePower { get; set; }
        public string ProductLevelSymbol { get; set; }
        public DateTime? CDD { get; set; }
        public DateTime? RDD { get; set; }
        public DateTime? UDD { get; set; }
        public DateTime? PBD { get; set; }
        public DateTime? DAC { get; set; }
        public DateTime? LCD { get; set; }
        public byte[] RowVersion { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}

namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class SalesOrderLine : Entity, IRowVersion
    {
        public long SalesOrderHeaderId { get; set; }
        public int SalesOrderLineTypeId { get; set; }
        public LineType SalesOrderLineType { get; set; }
        public long LocalModelId { get; set; }
        //public  LocalModel LocalModel { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPriceExclVAT { get; set; }
        public decimal SalesPriceInclVAT { get; set; }
        public decimal Cost { get; set; }
        public decimal? VatTaxRate { get; set; }
        public string VatTaxRateCode { get; set; }
        public string VatTaxRateDescription { get; set; }
        public int? PaymentCodeId { get; set; }
        public PaymentCode PaymentCode { get; set; }

        public long? SalesPurchaseOrderId { get; set; }
        public string SalesPurchaseOrderNumber { get; set; }
        public decimal? POValue { get; set; }
        public decimal? GRValue { get; set; }

        private SalesOrderLineVehicle salesOrderLineVehicle;
        public SalesOrderLineVehicle SalesOrderLineVehicle
        {
            get
            {
                if (SalesOrderLineTypeId != LineType.Vehicle.Id)
                {
                    return null;
                }
                return salesOrderLineVehicle;
            }
            set => salesOrderLineVehicle = value;
        }

        private SalesOrderLineExternalWorkshop externalWorkshop;
        public SalesOrderLineExternalWorkshop ExternalWorkshop
        {
            get
            {
                if (SalesOrderLineTypeId != PreparationType.ExternalWorkShop.Id)
                {
                    return null;
                }
                return externalWorkshop;
            }
            set => externalWorkshop = value;
        }

        private SalesOrderLineInternalWorkshop internalWorkshop;
        public SalesOrderLineInternalWorkshop InternalWorkshop
        {
            get
            {
                if (SalesOrderLineTypeId != PreparationType.InternalWorkShop.Id)
                {
                    return null;
                }
                return internalWorkshop;
            }
            set => internalWorkshop = value;
        }

        private SalesOrderLineAdditionalService additionalService;
        public SalesOrderLineAdditionalService AdditionalService
        {
            get
            {
                if (SalesOrderLineTypeId != PreparationType.AdditionalService.Id)
                {
                    return null;
                }
                return additionalService;
            }
            set => additionalService = value;
        }
        public List<SalesOrderLinePriceBreakDown> SalesOrderLinePriceBreakDowns { get; set; }
        public bool IsObsolete { get; set; }
        public long? SalesOrderLinePurchaseGroupId { get; set; }

        [JsonIgnore]
        public IPreparation Preparation
        {
            get
            {
                if (SalesOrderLineTypeId == PreparationType.CAWorkShop.Id)
                {
                    return CAWorkShop;
                }
                else if (SalesOrderLineTypeId == PreparationType.InternalWorkShop.Id)
                {
                    return InternalWorkshop;
                }
                else if (SalesOrderLineTypeId == PreparationType.ExternalWorkShop.Id)
                {
                    return ExternalWorkshop;
                }
                else if (SalesOrderLineTypeId == PreparationType.AdditionalService.Id)
                {
                    return AdditionalService;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (SalesOrderLineTypeId == PreparationType.CAWorkShop.Id)
                {
                    CAWorkShop = (SalesOrderLineCAWorkShop)value;
                }
                else if (SalesOrderLineTypeId == PreparationType.InternalWorkShop.Id)
                {
                    InternalWorkshop = (SalesOrderLineInternalWorkshop)value;
                }
                else if (SalesOrderLineTypeId == PreparationType.ExternalWorkShop.Id)
                {
                    ExternalWorkshop = (SalesOrderLineExternalWorkshop)value;
                }
                else if (SalesOrderLineTypeId == PreparationType.AdditionalService.Id)
                {
                    AdditionalService = (SalesOrderLineAdditionalService)value;
                }
            }
        }

        private SalesOrderLineCAWorkShop cAWorkShop;
        public SalesOrderLineCAWorkShop CAWorkShop
        {
            get
            {
                if (SalesOrderLineTypeId != PreparationType.CAWorkShop.Id)
                {
                    return null;
                }
                return cAWorkShop;
            }
            set => cAWorkShop = value;
        }
        public int? SequenceNo { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string Description { get; set; }
        public string ReferenceNumber { get; set; }

        public byte[] RowVersion { get; set; }

        public bool IsVehicleLine()
        {
            return SalesOrderLineTypeId == LineType.Vehicle.Id;
        }

        public bool IsPreparationLine()
        {
            return !IsVehicleLine();
        }
    }

}

public enum SalesOrderLinePriceBreakDownType
{
    Model = 1,
    Option = 2,
    CA = 3,
}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLinePriceBreakDown : Entity, IRowVersion
    {
        public long SalesOrderLineId { get; set; }
        public SalesOrderLinePriceBreakDownType Description { get; set; }
        public decimal ListPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal SalesPriceExclVAT { get; set; }
        public decimal SalesPriceInclVAT { get; set; }
        public decimal? VatTaxRate { get; set; }
        public string VatTaxRateCode { get; set; }
        public string VatTaxRateDescription { get; set; }
        public int? SequenceNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public byte[] RowVersion { get; set; }

    }

}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLineExternalWorkshop : Entity, IRowVersion, IPreparation
    {
        public string Group { get; set; }
        public string Description { get; set; }
        public string Supplier { get; set; }
        public int PreparationStatusId { get; set; }
        public PreparationStatus PreparationStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public long SalesOrderLineId { get; set; }
        public long? RefId { get; set; }
        public byte[] RowVersion { get; set; }
        public string Identifier { get; set; }
    }
}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLineInternalWorkshop : Entity, IRowVersion, IPreparation
    {
        public string Group { get; set; }
        public string Description { get; set; }
        public string Supplier { get; set; }
        public int PreparationStatusId { get; set; }
        public PreparationStatus PreparationStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public long SalesOrderLineId { get; set; }
        public long? RefId { get; set; }
        public byte[] RowVersion { get; set; }
        public string Identifier { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.EntityModel.SalesOrder
{
    public interface IPreparation
    {
        string Group { get; set; }
        string Description { get; set; }
        int PreparationStatusId { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? CompletedDate { get; set; }
        string Supplier { get; set; }
    }
}
public class PreparationStatus : Enumeration
{
    public static readonly PreparationStatus NotStarted = new PreparationStatus(1, nameof(NotStarted), 5);
    public static readonly PreparationStatus InProgress = new PreparationStatus(2, nameof(InProgress), 10);
    public static readonly PreparationStatus Complete = new PreparationStatus(3, nameof(Complete), 15);

    public int? SequenceNo { get; set; }
    public PreparationStatus(int id, string name, int? sequenceNo) : base(id, name)
    {
        SequenceNo = sequenceNo;
    }
}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLineAdditionalService : Entity, IRowVersion, IPreparation
    {
        public string Group { get; set; }
        public string Description { get; set; }
        public string Supplier { get; set; }
        public int PreparationStatusId { get; set; }
        public PreparationStatus PreparationStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public long SalesOrderLineId { get; set; }
        public long? RefId { get; set; }
        public byte[] RowVersion { get; set; }
        public string Identifier { get; set; }
    }
}
namespace LSS.VehicleSalesOrder.Model.Entities
{
    public class SalesOrderLineCAWorkShop : Entity, IRowVersion, IPreparation
    {
        public string Group { get; set; }
        public string Description { get; set; }
        public int PreparationStatusId { get; set; }
        public string Supplier { get; set; }
        public PreparationStatus PreparationStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public long SalesOrderLineId { get; set; }
        public long? RefId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class SalesOrderHeader
    {
        public long Id { get; set; }
        public int SalesModeId { get; set; }
        public SalesMode SalesMode { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string DealProposalNumber { get; set; }
        public DateTime? CPDD { get; set; }
        public DateTime? CPDDAtDAC { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliveryDateComment { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int? StageId { get; set; }
        public Stage Stage { get; set; }
        public string SalesPersonId { get; set; }
        public string SalesPersonName { get; set; }
        public string SalesManagerId { get; set; }
        public string SalesManagerName { get; set; }
        public string SalesAdminId { get; set; }
        public string SalesAdminName { get; set; }
        public string SalesAdminId2 { get; set; }
        public string SalesAdminName2 { get; set; }
        public long? DealerId { get; set; }
        public Dealer Dealer { get; set; }
        public long? CancelReasonId { get; set; }
        public CancelReason CancelReason { get; set; }
        public string CancelReasonComment { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTimeOffset LastModifiedDate { get; set; }
        public long? CustomerId { get; set; }
        public SalesOrderCustomer Customer { get; set; }

        public DateTime? CustomerOrderDate { get; set; }
        public List<SalesOrderLine> SalesOrderLines { get; set; }
        public string Notes { get; set; }
        public bool? IsCalculationConfirmed { get; set; }
        public string Segment { get; set; }
        public string OrderReferenceNo { get; set; }
        public bool IsDomestic { get; set; }
        public bool IsInternal { get; set; }
        public long? SalesTypeId { get; set; }
        public string SalesTypeName { get; set; }
        public int? PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public long? PaymentTermId { get; set; }
        public string PaymentTermDescription { get; set; }
        public string CurrencyCode { get; set; }
        public SalesOrderInvoiceTo InvoiceTo { get; set; }
        public long MarketId { get; set; }
        public long? ProposalId { get; set; }
        public int? FloorPlanFreeDays { get; set; }
        public decimal? InterestRate { get; set; }
        public string HomologationModel { get; set; }
        public string Source { get; set; }
        public byte[] RowVersion { get; set; }
        public List<SalesOrderDeliveryAddress> SalesOrderDeliveryAddresses { get; set; }
        #region Invoice 
        public long? InvoiceHeaderId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? TotalInvoiceAmountInclVat { get; set; }
        public decimal? TotalInvoiceAmountExclVat { get; set; }
        public int? InvoiceTypeId { get; set; }
        public int? InvoiceStatusId { get; set; }
        #endregion
        #region Incoterm session
        public long? IncotermId { get; set; }
        public Incoterm Incoterm { get; set; }
        public string DeliveryTermPlace { get; set; }
        #endregion
        public string SalesDistrict { get; set; }
        public DateTimeOffset? RetailDate { get; set; }
        public bool? IsBlockAutoInvoice { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Enums
{
    public enum InvoiceToType
    {
        Dealer = 1,
        Customer = 2,
        Institute = 3
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Values
{
    public class SalesOrderInvoiceTo
    {
        public long Id { get; set; }
        public long SalesOrderHeaderId { get; set; }
        public InvoiceToType InvoiceToType { get; set; }
        public string BusinessKey { get; set; }
        public string Name { get; set; }
        public string TelephoneNumber { get; set; }
        public string ParmaId { get; set; }
        public string FaxNumber { get; set; }
        public string VatNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public long? PartnerTypeId { get; set; }
        public PartnerType PartnerType { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.EntityModel
{
    public class PartnerType
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? SequenceNo { get; set; }
        public long MarketId { get; set; }
    }
}

namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class CancelReason : Entity<int>
    {
        public string Name { get; set; }
        public long MarketId { get; set; }
        public Market Market { get; set; }
        public bool IsDefault { get; set; }
        /// <summary>
        /// For Credit Note of TSA Factory, will set up as true for cancel order purpose
        /// </summary>
        public virtual bool Internal { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Models
{
    [Serializable]
    public class Market
    {
        public long Id { get; set; }
        public int MarketId { get; set; }
        public string Description { get; set; }
        public string CountryCode { get; set; }
        public string PreferedCulture { get; set; }
        public string TimeZoneId { get; set; }
    }
}
namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.EntityModel
{
    public class Incoterm : Entity
    {
        public virtual long MarketId { get; set; }
        public virtual Market Market { get; set; }
        public virtual string IncotermVersion { get; set; }
        public virtual string TermsOfDelivery { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int Sort { get; set; }
        public virtual byte[] RowVersion { get; set; }
    }
}

namespace LSS.VehicleIntegrationTransaction.Model.Enums
{
    public class SalesMode : Enumeration
    {
        public static SalesMode Direct = new SalesMode(1, nameof(Direct));
        public static SalesMode DealerSales = new SalesMode(2, nameof(DealerSales));
        public SalesMode(int id, string name) : base(id, name)
        {
        }
    }
}
namespace LSS.VehicleIntegrationTransaction.Model.Commons
{
    public abstract class Enumeration : Entity<int>, IComparable
    {
        public string Name { get; private set; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Where(type => type.FieldType.IsSubclassOf(typeof(Enumeration)))
                .Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object other)
        {
            var otherValue = other as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(other.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }

        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }

        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }

        public int CompareTo(object obj) => Id.CompareTo(((Enumeration)obj).Id);
    }
}

namespace NVS.Storage
{
//
// Summary:
//     Provides a base and abstract entity implementation.
//
// Type parameters:
//   TId:
//     A type of an entity identifier.
//
// Remarks:
//     The base and abstract NVS.Storage.Entity`1 class provides functionality which
//     is needed for most of the entities. It gives the NVS.Storage.Entity`1.Id property
//     acting like an entity identifier. In standard cases, you don't assign the NVS.Storage.Entity`1.Id
//     manually but it's set by a selected ORM mapper and framework.
//
//     An entity can be transient (not stored yet) or persisted. In order to check if
//     an entity is transient the NVS.Storage.Entity`1.IsTransient function can be used.
//     By default, an entity is transient when it has no value for its NVS.Storage.Entity`1.Id
//     (its identifier holds a default value, default(TId)).
//
//     The NVS.Storage.Entity`1 class gives also a default implementation of the equality
//     and hash code functions.
//
//     When two different entity objects (two different object instances) are checked
//     for equality then they are recognized as being equal if and only if they are
//     of the same type, they are not transient (meaning that they are persisted) and
//     they have equal (the same) entity identifiers (NVS.Storage.Entity`1.Ids). The
//     equality comparison will also return true when trying to compare exactly the
//     same objects (when trying to compare the same object references).
//
//     When the NVS.Storage.Entity`1 generates a hash code via the NVS.Storage.Entity`1.GetHashCode
//     function, a different hash code can be created for a transient and for a persisted
//     entity. For transient entities, an object reference is used. For persisted entities,
//     an entity identifier is utilized. Once a hash code is calculated, it will be
//     re-used for the current instance.
public abstract class Entity<TId> where TId : notnull
{
    //
    // Summary:
    //     An already calculated hash code for this entity.
    private int? hashCode;

    //
    // Summary:
    //     An entity identifier.
    public virtual TId Id
    {
        [return: MaybeNull]
        get;
        [param: AllowNull]
        set;
    }

    //
    // Summary:
    //     Determines if an entity is transient (not persisted yet).
    //
    // Returns:
    //     true if an entity is transient, false if entity is persisted.
    public virtual bool IsTransient()
    {
        if (Id != null)
        {
            return Id.Equals(default(TId));
        }

        return true;
    }

    //
    // Summary:
    //     Determines whether a given entity object is equal to the current one.
    //
    // Parameters:
    //   obj:
    //     An entity object which should be compared to the current one.
    //
    // Returns:
    //     true if entities are recognized as being equal.
    //
    // Remarks:
    //     When two different entity objects are checked for equality then they are recognized
    //     as being equal if they are of the same type, they are not transient (meaning
    //     that they are persisted) and they have equal (the same) entity identifiers.
    //
    //     They are also equal if they are exactly the same objects (the same references).
    public override bool Equals(object? obj)
    {
        if (!(obj is Entity<TId> entity))
        {
            return false;
        }

        if (this == obj)
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        if (entity.IsTransient() || IsTransient())
        {
            return false;
        }

        return entity.Id.Equals(Id);
    }

    //
    // Summary:
    //     Checks if two entities are equal. See NVS.Storage.Entity`1.Equals(System.Object)
    //     for more information.
    //
    // Parameters:
    //   left:
    //     A left entity to be checked.
    //
    //   right:
    //     A right entity to be checked.
    //
    // Returns:
    //     true if two entity objects are recognized as being equal.
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (!object.Equals(left, null))
        {
            return left.Equals(right);
        }

        return object.Equals(right, null);
    }

    //
    // Summary:
    //     Checks if two entities are not equal. See NVS.Storage.Entity`1.Equals(System.Object)
    //     for more information.
    //
    // Parameters:
    //   left:
    //     A left entity to be checked.
    //
    //   right:
    //     A right entity to be checked.
    //
    // Returns:
    //     true if two entity objects are recognized as being equal.
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }

    //
    // Summary:
    //     Generates a hashcode used for the hashset distribution.
    //
    // Remarks:
    //     The hashcode generated by the class depends on an entity transient state. For
    //     transient entities an object reference is used. For persisted entities an entity
    //     identifier is utilized.
    public override int GetHashCode()
    {
        if (hashCode.HasValue)
        {
            return hashCode.Value;
        }

        hashCode = (IsTransient() ? base.GetHashCode() : HashCode.Combine(Id));
        return hashCode.Value;
    }
}}


namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class Dealer
    {
        public long Id { get; set; }
        public string DealerId { get; set; }
        public string DealerName { get; set; }
        public string Prefix { get; set; }
        public string TelephoneNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Alias { get; set; }
        public string VATNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameLocal { get; set; }
        public string CompanyRegNumber { get; set; }
        public string CountryCode { get; set; }
        public string InvoiceCurrency { get; set; }
        public int DealerType { get; set; }
        public bool OwnedDealer { get; set; }
        public string DeliveryTermPlace { get; set; }
        public string MainDealerId { get; set; }
        public string ContactName { get; set; }
        public string ContactTelephoneNumber { get; set; }
        public long? SalesRegionId { get; set; }
#nullable enable // Enable nullable reference types
        public SalesRegion? SalesRegion { get; set; }
#nullable restore // Restore the nullable context to its previous state
        public string SalesRegionCode { get; set; }
        public string SalesRegionName { get; set; }
        public string SalesRegionCompanyCode { get; set; }
        public int MarketId { get; set; }
        public string ParmaId { get; set; }
        public string PartyId { get; set; }
        public string ExternalDealerCode1 { get; set; }
        public string ExternalDealerCode2 { get; set; }
        public string DefaultTermofDelivery { get; set; }
        public string DefaultIncotermVersion { get; set; }
        public string DefaultInvoiceTo { get; set; }
        public string DefaultInvoiceToName { get; set; }
        public string DefaultSendTo { get; set; }
        public string DefaultInvoiceDeclaration { get; set; }

        public List<DealerAddress> Addresses { get; set; }
        public bool? IsActive { get; set; }
        public string MainAddress { get; set; }
        public string VisitAddress { get; set; }
        #region not master data
        public long? PaymentTermId { get; set; }
        public string PaymentTermDescription { get; set; }
        public int? PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentTermCode { get; set; }
        public long? PartnerTypeId { get; set; }
        public string DefaultFinancialInstituteCode { get; set; }
        public string TradingPartner { get; set; }
        #endregion

        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
        public byte[] RowVersion { get; set; }
        public string DealerDescription => $"{DealerId}-{DealerName}";
    }
}

namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class SalesRegion
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }
        public long MarketId { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string LastModifiedBy { get; set; }
    }
}

namespace LSS.VehicleIntegrationTransaction.SalesOrder.Model.MessageModel.SalesOrderMessage
{
    public class DealerAddress
    {
        public string AddressDetail { get; set; }
        public string AddressDetailLocal { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CityLocal { get; set; }
        public string Region { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public long DealerId { get; set; }
        public int? AddressTypeId { get; set; }
    }
}