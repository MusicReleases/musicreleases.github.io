namespace JakubKastner.MusicReleases.Services.ApiServices;

using System.Threading.Channels;

public static class RequestScheduler
{
	public static async Task Run<TItem>(
		IEnumerable<TItem> items,
		int degreeOfParallelism,
		Func<TItem, CancellationToken, Task> work,
		CancellationToken ct,
		Func<int, int, Task>? onProgress = null)
	{
		var list = items as IList<TItem> ?? items.ToList();
		var total = list.Count;
		var done = 0;

		// bounded channel = backpressure
		var channel = Channel.CreateBounded<TItem>(new BoundedChannelOptions(degreeOfParallelism * 2)
		{
			SingleWriter = true,
			SingleReader = false,
			FullMode = BoundedChannelFullMode.Wait
		});

		// start workers
		var workers = Enumerable.Range(0, degreeOfParallelism).Select(async _ =>
		{
			await foreach (var item in channel.Reader.ReadAllAsync(ct))
			{
				await work(item, ct);

				var finished = Interlocked.Increment(ref done);
				if (onProgress is not null)
					await onProgress(finished, total);
			}
		}).ToArray();

		// feed channel
		foreach (var item in list)
		{
			ct.ThrowIfCancellationRequested();
			await channel.Writer.WriteAsync(item, ct);
		}
		channel.Writer.Complete();

		await Task.WhenAll(workers);
	}
}