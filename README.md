# Json2CSharpExtension
Json2CSharp Extension for Visual Studio 2019


For example:
```json
{
	"creditInfo": {
		"account": "4230677804",
		"status": "ΕΝΕΡΓΟΣ             ",
		"product": {
			"code": "01414101Β01",
			"description": "Description"
		},
		"accountType": "ΚΑΤΑΝΑΛΩΤΙΚΟ        ",
		"accountOverdueStatus": "ΕΝΗΜΕΡΟ",
		"branch": "0040"
	}
}
```
To :
```csharp
{
	CreditInfo = new CreditInfo
	{
		Account = "4230677804",
		Status = "ΕΝΕΡΓΟΣ             ",
		Product = new Product
		{
			Code = "01414101Β01",
			Description = "Description"
		},
		AccountType = "ΚΑΤΑΝΑΛΩΤΙΚΟ        ",
		AccountOverdueStatus = "ΕΝΗΜΕΡΟ",
		Branch = "0040"
	}
}
```
**You have to copy a json string and then from menu navigate to "Edit" -> "Paste Json as C# Code".**
