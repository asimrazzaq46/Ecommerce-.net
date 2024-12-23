namespace Core.Params;

public class ProductSpecsSearchParams
{

	private const int MaxpageSize = 10;

	public int pageIndex { get; set; } = 1;


	private int _pageSize = 5 ;
	public int pageSize
    {
		get { return _pageSize; }
		set { _pageSize = value > MaxpageSize ? _pageSize : value; }

	}

	private List<string> _brands = []; //brands=react,angular transform into ["react","angular"]
	public List<string> Brands
	{
		get => _brands;
		set { _brands = value.SelectMany(x=>x.Split(",",StringSplitOptions.RemoveEmptyEntries)).ToList(); }
	}
 
	private List<string> _types = [];  //types=boards,tyre transform into ["boards","tyre"]
    public List<string> Types
	{
		get { return _types; }
		set { _types = value.SelectMany(x => x.Split(",", StringSplitOptions.RemoveEmptyEntries)).ToList(); }
	}

    public string? Sort { get; set; }

	private string? _serach;

	public string Search
	{
		get => _serach ?? "";
		set => _serach = value.ToLower(); 
	}




}
