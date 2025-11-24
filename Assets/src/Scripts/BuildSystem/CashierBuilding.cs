public class CashierBuilding : BuildingObjectBase
{
    protected override void ProcessCustomerReward()
    {
        if (buildingData.ProfitPerCustomer > 0)
        {
            CurrencyController.Instance.AddCurrency(buildingData.ProfitPerCustomer);
        }
    }
}