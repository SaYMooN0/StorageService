using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.services;
using StorageService.Domain.common;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;
using StorageService.Domain.entities.transport_company;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api;

public class AppDataSeeder
{
    public static async Task SeedInitialData(AppDbContext dbContext, PasswordHasher hasher) {
        // уже есть данные
        if (await dbContext.Admins.AnyAsync()) {
            return;
        }


        var admin = CompanyAdmin.CreateNew("bAdmin", hasher.HashPassword("abcd1234")).AsSuccess();
        var company = TransportCompany.CreateNew("TestTransCo", admin.Id).AsSuccess();

        var product1Props = new Dictionary<string, string> {
            ["CPU"] = "Intel i7",
            ["Battery"] = "10h"
        }.ToImmutableDictionary();
        var product1 = Product.CreateNew("Laptop Pro", "High-end model", product1Props).AsSuccess();

        var product2Props = new Dictionary<string, string> {
            ["Clicks"] = "10 million",
            ["Type"] = "Wired"
        }.ToImmutableDictionary();
        var product2 = Product.CreateNew("Mouse 2000", "Ergonomic mouse", product2Props).AsSuccess();

        var product3Props = new Dictionary<string, string> {
            ["Длина"] = "50cm",
            ["Материал"] = "Пластик",
            ["Степень мемности"] = "9000"
        }.ToImmutableDictionary();
        var product3 = Product.CreateNew("Мемная указка 'Палец'",
            "Классическая пластиковая указка в виде гигантского пальца", product3Props).AsSuccess();

        var product4Props = new Dictionary<string, string> {
            ["Подсветка"] = "RGB",
            ["Мощность"] = "750W",
            ["Совместимость"] = "RTX 4090"
        }.ToImmutableDictionary();
        var product4 = Product.CreateNew("RGB Тостер", "Геймерский тостер с подсветкой", product4Props).AsSuccess();

        var product5Props = new Dictionary<string, string> {
            ["Объём"] = "300ml",
            ["Скорость охлаждения"] = "медленная",
            ["Мемность"] = "высокая"
        }.ToImmutableDictionary();
        var product5 = Product.CreateNew("Холодильник для чая", null, product5Props).AsSuccess();

        var product6Props = new Dictionary<string, string> {
            ["Позиций наклона"] = "12",
            ["Встроенный кофе"] = "Нет",
            ["Цвет"] = "Чёрный (и немного отчаяния)"
        }.ToImmutableDictionary();
        var product6 = Product.CreateNew("Стул разработчика", null, product6Props).AsSuccess();

        var product7Props = new Dictionary<string, string> {
            ["Ошибок в день"] = "50",
            ["Режим паники"] = "Да",
            ["Тип подключения"] = "Bluetooth (иногда)"
        }.ToImmutableDictionary();
        var product7 = Product
            .CreateNew("Мышь 'Я устал'", "Каждое пятое нажатие кликает в случайную точку", product7Props).AsSuccess();

        var product8Props = new Dictionary<string, string> {
            ["Размер"] = "XL",
            ["Команды"] = "i, :wq, dd, ggVGd",
            ["Материал"] = "антибаговая резина"
        }.ToImmutableDictionary();
        var product8 = Product
            .CreateNew("Коврик с vim-командами", "Помогает не забыть, как выйти из vim", product8Props).AsSuccess();

        var product9 = Product.CreateNew("Термокружка DevOps'а", "Не прольёт даже под CI/CD стрессом",
            ImmutableDictionary<string, string>.Empty
        ).AsSuccess();

        var product10 = Product.CreateNew("Антистресс-банан", "Жмяк-жмяк, и тревога отходит",
            ImmutableDictionary<string, string>.Empty
        ).AsSuccess();

        var storage1 = Storage.CreateNew("Главное хранилище", admin.Id).AsSuccess();
        var storage2 = Storage.CreateNew("Backup Storage", admin.Id).AsSuccess();

        storage1.AddProduct(product1.Id, 5);
        storage1.AddProduct(product2.Id, 10);
        storage1.AddProduct(product3.Id, 22);
        storage1.AddProduct(product4.Id, 27);
        storage1.AddProduct(product5.Id, 18);

        var transportRecord = TransportationRecord.CreateNew(
            company.Id,
            storage1.Id,
            storage2.Id,
            new Dictionary<ProductId, uint> {
                [product1.Id] = 2,
                [product2.Id] = 5
            }
        );

        await dbContext.Admins.AddAsync(admin);
        await dbContext.TransportCompanies.AddAsync(company);
        await dbContext.Products.AddRangeAsync(product1, product2, product3, product4, product5, product6, product7,
            product8, product9, product10);
        await dbContext.Storages.AddRangeAsync(storage1, storage2);
        await dbContext.TransportationRecords.AddAsync(transportRecord);

        await dbContext.SaveChangesAsync();
    }
}