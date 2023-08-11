
namespace AutoMockFixture;

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class CallBaseAttribute : Attribute
{
    public CallBaseAttribute()
    {
        CallBase = true;
    }

    public CallBaseAttribute(bool callBase)
	{
		CallBase = callBase;
	}

	public bool CallBase { get; set; }
}
