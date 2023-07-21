namespace Application.Core.Request;

public sealed class SSFQueryParams {
	public int? LastItemId { get; set; }
	public int PageSize { get; set; } = 10;

	// Search Sort Filter
	public string? Search { get; set; }
	
	public override string ToString() {
		return $"SSFQueryParams(LastItemId={LastItemId}, PageSize={PageSize}, Search={Search})";
	}
}
