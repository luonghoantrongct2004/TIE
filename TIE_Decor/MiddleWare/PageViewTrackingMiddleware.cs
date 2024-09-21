using TIE_Decor.Service;

namespace TIE_Decor.MiddleWare;
public class PageViewTrackingMiddleware
{
    private readonly RequestDelegate _next;

    public PageViewTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Kiểm tra nếu URL không bắt đầu bằng "/admin" thì mới theo dõi lượt xem
        if (!context.Request.Path.StartsWithSegments("/admin"))
        {
            var trackingService = context.RequestServices.GetRequiredService<ITrackingService>();
            string path = context.Request.Path;

            // Ghi lại lượt xem nếu không phải là trang admin
            await trackingService.TrackPageView(path);
        }

        // Tiếp tục xử lý request bình thường
        await _next(context);
    }
}
