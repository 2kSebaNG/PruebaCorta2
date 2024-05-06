using chairs_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS
chairs.MapPost("/", CreateChair);
chairs.MapGet("/", GetChairs);
chairs.MapGet("/{nombre}",GetChairByName);
chairs.MapPut("/{id}", UpdateChair);
chairs.MapPut("/{id}/stock",IncreaseStock);
chairs.MapPost("/purchase", PurchaseChair);
chairs.MapDelete("/{id}", DeleteChair);

app.Run();

//TODO: ENDPOINTS SOLICITADOS
static IResult GetChairs(DataContext db)
{
    return TypedResults.Ok(db.Chairs.ToList());
}

static IResult CreateChair(Chair chair, DataContext db)
{
    var newChair = db.Chairs.FirstOrDefault(c => c.Nombre == chair.Nombre);

    if(newChair is not null){
        throw new Exception();
    }

    db.Chairs.Add(chair);
    db.SaveChanges();

    return TypedResults.Created($"/chairs/{chair.Id}", chair);
}

static IResult GetChairByName(string nombre, DataContext db){

    var chair = db.Chairs.FirstOrDefault(c => c.Nombre == nombre);

    if(chair is null){
        return TypedResults.NotFound();
    }

    return TypedResults.Ok(chair);
}

static IResult UpdateChair(int id, Chair inputChair,DataContext db){

    var chair = db.Chairs.Find(id);

    if(chair is null){
        return TypedResults.NotFound();
    }

    chair.Nombre = inputChair.Nombre;
    chair.Tipo = inputChair.Tipo;
    chair.Material = inputChair.Material;
    chair.Color = inputChair.Color;
    chair.Altura = inputChair.Altura;
    chair.Anchura = inputChair.Anchura;
    chair.Profundidad = inputChair.Profundidad;
    chair.Precio = inputChair.Precio;

    db.SaveChanges();

    return TypedResults.NoContent();
}

static IResult IncreaseStock(int id, int increase, DataContext db){

    var chair = db.Chairs.Find(id);

    if(chair is null){
        return TypedResults.NotFound();
    }

    chair.Stock += increase;

    db.SaveChanges();

    return TypedResults.Ok(chair);

}

static IResult PurchaseChair(int id, int quantity, int totalPrice, DataContext db){

    var chair = db.Chairs.Find(id);

    if(chair is null){
        return TypedResults.BadRequest("La silla con ese id no existe");
    }

    if(chair.Stock < quantity){

        return TypedResults.BadRequest("La cantidad supera el stock de la silla");
    }

    var verifyPrice = quantity*chair.Precio;

    if(verifyPrice != totalPrice){
        return TypedResults.BadRequest("El precio total ingresado no corresponde con el precio real");
    }

    chair.Stock -= quantity;

    db.SaveChanges();

    return TypedResults.Ok("Su compra se ha realizado con éxito");

}


static IResult DeleteChair(int id, DataContext db){

    var chair = db.Chairs.Find(id);

    if(chair is null){
        return TypedResults.NotFound();
    }

    db.Chairs.Remove(chair);
    db.SaveChanges();

    return TypedResults.NoContent();

}