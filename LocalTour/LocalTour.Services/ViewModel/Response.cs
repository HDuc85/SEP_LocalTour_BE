namespace LocalTour.Services.ViewModel;

public record Response(
    int error,
    String message,
    object? data
);