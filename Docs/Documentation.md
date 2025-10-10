# 📚 Документация проекта Animal Shelter Management System

## Содержание

- [1. Обзор проекта](#1-обзор-проекта)
- [2. Архитектура базы данных](#2-архитектура-базы-данных)
- [3. Структура проекта](#3-структура-проекта)
- [4. Модели данных (Models)](#4-модели-данных-models)
- [5. Слой доступа к данным (Data)](#5-слой-доступа-к-данным-data)
- [6. Репозитории (Repositories)](#6-репозитории-repositories)
- [7. Бизнес-логика (Services)](#7-бизнес-логика-services)
- [8. Интерфейсы (Interfaces)](#8-интерфейсы-interfaces)
- [9. Вспомогательные классы (Helpers)](#9-вспомогательные-классы-helpers)
- [10. Главное приложение (Program.cs)](#10-главное-приложение-programcs)
- [11. Нормализация базы данных](#11-нормализация-базы-данных)
- [12. Установка и запуск](#12-установка-и-запуск)

---

## 1. Обзор проекта

**Animal Shelter Management System** — консольное приложение для управления приютом для животных, разработанное на C# с использованием Entity Framework Core и PostgreSQL.

### Основные возможности

- ✅ Управление животными (CRUD операции)
- ✅ Регистрация и управление усыновителями
- ✅ Обработка заявок на усыновление
- ✅ Ведение медицинских карт и истории вакцинаций
- ✅ Отчеты и выборки с фильтрацией
- ✅ Статистика по приюту

### Технологический стек

- **Язык**: C# 12
- **Фреймворк**: .NET 9.0
- **ORM**: Entity Framework Core 9.0
- **База данных**: PostgreSQL
- **Провайдер**: Npgsql.EntityFrameworkCore.PostgreSQL
- **Паттерны**: Repository, Service Layer

---

## 2. Архитектура базы данных

### ER-диаграмма

![ER-диаграмма базы данных](telegram-cloud-photo-size-2-5422432439705404352-y.jpg)

### Описание таблиц

#### 1. **adopters** (Усыновители)
Хранит информацию о людях, желающих усыновить животных.

| Поле | Тип | Описание |
|------|-----|----------|
| adopter_id | SERIAL (PK) | Уникальный идентификатор |
| first_name | VARCHAR(100) | Имя |
| last_name | VARCHAR(100) | Фамилия |
| email | VARCHAR(100) UNIQUE | Email (уникальный) |
| phone | VARCHAR(20) | Телефон |
| address | TEXT | Адрес |
| registration_date | DATE | Дата регистрации |

#### 2. **animals** (Животные)
Содержит данные о животных в приюте.

| Поле | Тип | Описание |
|------|-----|----------|
| animal_id | SERIAL (PK) | Уникальный идентификатор |
| name | VARCHAR(100) | Кличка животного |
| age | INTEGER | Возраст |
| species | VARCHAR(50) | Вид (собака, кошка и т.д.) |
| breed | VARCHAR(100) | Порода |
| gender | VARCHAR(10) | Пол |
| date_admitted | DATE | Дата поступления в приют |
| status_id | INTEGER (FK) | Статус животного |
| description | TEXT | Описание |

**Связь с animal_status**: 1:M (одному статусу соответствует много животных)

#### 3. **animal_status** (Статусы животных)
Справочник статусов животных.

| Поле | Тип | Описание |
|------|-----|----------|
| status_id | SERIAL (PK) | Уникальный идентификатор |
| status_name | VARCHAR(50) UNIQUE | Название статуса |

**Значения**: InShelter, Adopted, Treatment, Quarantine

#### 4. **medical_records** (Медицинские карты)
Медицинская информация о каждом животном.

| Поле | Тип | Описание |
|------|-----|----------|
| record_id | SERIAL (PK) | Уникальный идентификатор |
| animal_id | INTEGER (FK) | ID животного |
| owner_id | INTEGER (FK) | ID владельца (усыновителя) |
| blood_type | VARCHAR(10) | Группа крови |
| allergies | TEXT | Аллергии |
| chronic_conditions | TEXT | Хронические заболевания |
| special_needs | TEXT | Особые потребности |
| veterinarian_notes | TEXT | Заметки ветеринара |
| last_checkup_date | DATE | Дата последнего осмотра |
| created_date | DATE | Дата создания карты |

**Связи**: 
- 1:1 с animals (одна карта на одно животное)
- M:1 с adopters через owner_id

#### 5. **adoptions** (Усыновления)
Заявки и история усыновлений.

| Поле | Тип | Описание |
|------|-----|----------|
| adoption_id | SERIAL (PK) | Уникальный идентификатор |
| adopter_id | INTEGER (FK) | ID усыновителя |
| animal_id | INTEGER (FK) | ID животного |
| adoption_date | DATE | Дата усыновления |
| status_id | INTEGER (FK) | Статус заявки |
| return_date | DATE | Дата возврата (если был) |
| description | TEXT | Примечания |

**Связи**:
- M:1 с adopters (один усыновитель — много заявок)
- M:1 с animals (одно животное — много заявок)
- M:1 с adoption_status

#### 6. **adoption_status** (Статусы усыновлений)
Справочник статусов заявок.

| Поле | Тип | Описание |
|------|-----|----------|
| status_id | SERIAL (PK) | Уникальный идентификатор |
| status_name | VARCHAR(50) UNIQUE | Название статуса |

**Значения**: Pending, Approved, Rejected, Cancelled

#### 7. **vaccinations** (Справочник вакцин)
Каталог доступных вакцин.

| Поле | Тип | Описание |
|------|-----|----------|
| vaccination_id | SERIAL (PK) | Уникальный идентификатор |
| name | VARCHAR(100) UNIQUE | Название вакцины |
| description | TEXT | Описание |
| manufacturer | VARCHAR(100) | Производитель |
| validity_period | INTEGER | Срок действия (месяцы) |

#### 8. **animal_vaccinations** (История вакцинаций)
Связь между животными и вакцинами (M:N).

| Поле | Тип | Описание |
|------|-----|----------|
| vaccination_id | SERIAL (PK) | Уникальный идентификатор |
| animal_id | INTEGER (FK) | ID животного |
| vaccination_id | INTEGER (FK) | ID вакцины |
| vaccination_date | DATE | Дата вакцинации |
| created_date | DATE | Дата записи |

**Связи**:
- M:1 с animals
- M:1 с vaccinations

### Типы связей в базе данных

- **1:1** - animals ↔ medical_records (одно животное — одна мед. карта)
- **1:M** - adopters → adoptions (один усыновитель — много заявок)
- **1:M** - animals → adoptions (одно животное — много заявок)
- **1:M** - animal_status → animals (один статус — много животных)
- **1:M** - adoption_status → adoptions (один статус — много заявок)
- **M:N** - animals ↔ vaccinations (через animal_vaccinations)

---

## 3. Структура проекта

```
AnimalShelter/
│
├── 📂 Models/                      # Классы сущностей
│   ├── Adopter.cs                  # Модель усыновителя
│   ├── Animal.cs                   # Модель животного
│   ├── AnimalStatus.cs             # Модель статуса животного
│   ├── Adoption.cs                 # Модель усыновления
│   ├── AdoptionStatus.cs           # Модель статуса усыновления
│   ├── MedicalRecord.cs            # Модель медицинской карты
│   ├── Vaccination.cs              # Модель вакцины
│   └── AnimalVaccination.cs        # Модель истории вакцинации
│
├── 📂 Data/                        # Контекст базы данных
│   ├── AnimalShelterContext.cs     # DbContext с конфигурацией
│   └── DbContextFactory.cs         # Фабрика для создания контекста
│
├── 📂 Interfaces/                  # Интерфейсы
│   ├── IRepository.cs              # Базовый интерфейс репозитория
│   ├── IAnimalRepository.cs        # Интерфейс репозитория животных
│   ├── IAdopterRepository.cs       # Интерфейс репозитория усыновителей
│   └── IAdoptionRepository.cs      # Интерфейс репозитория усыновлений
│
├── 📂 Repositories/                # Реализация репозиториев
│   ├── Repository.cs               # Базовый репозиторий
│   ├── AnimalRepository.cs         # Репозиторий животных
│   ├── AdopterRepository.cs        # Репозиторий усыновителей
│   └── AdoptionRepository.cs       # Репозиторий усыновлений
│
├── 📂 Services/                    # Бизнес-логика
│   ├── AnimalService.cs            # Сервис управления животными
│   ├── AdopterService.cs           # Сервис управления усыновителями
│   ├── AdoptionService.cs          # Сервис управления усыновлениями
│   └── ReportService.cs            # Сервис отчетов и аналитики
│
├── 📂 Helpers/                     # Вспомогательные классы
│   └── ConsoleHelper.cs            # Утилиты для работы с консолью
│
├── 📄 Program.cs                   # Точка входа в приложение
├── 📄 MyFirstApp.csproj            # Файл проекта
└── 📄 README.md                    # Описание проекта
```

---

## 4. Модели данных (Models)

### 4.1 Adopter.cs

Класс для представления усыновителя.

```csharp
[Table("adopters")]
public class Adopter
{
    [Key]
    [Column("adopter_id")]
    public int AdopterId { get; set; }

    [Required]
    [Column("first_name")]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [Column("last_name")]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Required]
    [Column("email")]
    [MaxLength(100)]
    public string Email { get; set; }

    [Column("phone")]
    [MaxLength(20)]
    public string Phone { get; set; }

    [Column("address")]
    public string Address { get; set; }

    [Column("registration_date")]
    public DateTime RegistrationDate { get; set; }

    // Навигационное свойство
    public virtual ICollection<Adoption> Adoptions { get; set; }
}
```

**Ключевые особенности:**
- Первичный ключ: `adopter_id`
- Уникальное поле: `email`
- Связь 1:M с таблицей `adoptions`

### 4.2 Animal.cs

Класс для представления животного.

```csharp
[Table("animals")]
public class Animal
{
    [Key]
    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Column("age")]
    public int? Age { get; set; }

    [Required]
    [Column("species")]
    [MaxLength(50)]
    public string Species { get; set; }

    [Column("breed")]
    [MaxLength(100)]
    public string Breed { get; set; }

    [Column("gender")]
    [MaxLength(10)]
    public string Gender { get; set; }

    [Column("date_admitted")]
    public DateTime DateAdmitted { get; set; }

    [Required]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("description")]
    public string Description { get; set; }

    // Навигационные свойства
    [ForeignKey("StatusId")]
    public virtual AnimalStatus Status { get; set; }

    public virtual MedicalRecord MedicalRecord { get; set; }
    public virtual ICollection<Adoption> Adoptions { get; set; }
}
```

**Ключевые особенности:**
- Первичный ключ: `animal_id`
- Внешний ключ: `status_id` → `animal_status`
- Связи: 1:1 с `medical_records`, 1:M с `adoptions`

### 4.3 AnimalStatus.cs

Справочник статусов животных.

```csharp
[Table("animal_status")]
public class AnimalStatus
{
    [Key]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Required]
    [Column("status_name")]
    [MaxLength(50)]
    public string StatusName { get; set; }

    // Навигационное свойство
    public virtual ICollection<Animal> Animals { get; set; }
}
```

**Возможные значения:**
- InShelter (В приюте)
- Adopted (Усыновлён)
- Treatment (На лечении)
- Quarantine (Карантин)

### 4.4 MedicalRecord.cs

Медицинская карта животного.

```csharp
[Table("medical_records")]
public class MedicalRecord
{
    [Key]
    [Column("record_id")]
    public int RecordId { get; set; }

    [Required]
    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("owner_id")]
    public int? OwnerId { get; set; }

    [Column("blood_type")]
    [MaxLength(10)]
    public string BloodType { get; set; }

    [Column("allergies")]
    public string Allergies { get; set; }

    [Column("chronic_conditions")]
    public string ChronicConditions { get; set; }

    [Column("special_needs")]
    public string SpecialNeeds { get; set; }

    [Column("veterinarian_notes")]
    public string VeterinarianNotes { get; set; }

    [Column("last_checkup_date")]
    public DateTime? LastCheckupDate { get; set; }

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    // Навигационные свойства
    [ForeignKey("AnimalId")]
    public virtual Animal Animal { get; set; }

    [ForeignKey("OwnerId")]
    public virtual Adopter Owner { get; set; }

    public virtual ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
}
```

**Ключевые особенности:**
- Первичный ключ: `record_id`
- Внешние ключи: `animal_id`, `owner_id`
- Связь 1:1 с `animals`, M:1 с `adopters`

### 4.5 Adoption.cs

Заявка на усыновление.

```csharp
[Table("adoptions")]
public class Adoption
{
    [Key]
    [Column("adoption_id")]
    public int AdoptionId { get; set; }

    [Required]
    [Column("adopter_id")]
    public int AdopterId { get; set; }

    [Required]
    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Column("adoption_date")]
    public DateTime AdoptionDate { get; set; }

    [Required]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Column("return_date")]
    public DateTime? ReturnDate { get; set; }

    [Column("description")]
    public string Description { get; set; }

    // Навигационные свойства
    [ForeignKey("AdopterId")]
    public virtual Adopter Adopter { get; set; }

    [ForeignKey("AnimalId")]
    public virtual Animal Animal { get; set; }

    [ForeignKey("StatusId")]
    public virtual AdoptionStatus Status { get; set; }
}
```

### 4.6 AdoptionStatus.cs

Справочник статусов заявок на усыновление.

```csharp
[Table("adoption_status")]
public class AdoptionStatus
{
    [Key]
    [Column("status_id")]
    public int StatusId { get; set; }

    [Required]
    [Column("status_name")]
    [MaxLength(50)]
    public string StatusName { get; set; }

    // Навигационное свойство
    public virtual ICollection<Adoption> Adoptions { get; set; }
}
```

**Возможные значения:**
- Pending (Ожидает рассмотрения)
- Approved (Одобрено)
- Rejected (Отклонено)
- Cancelled (Отменено)

### 4.7 Vaccination.cs

Справочник вакцин.

```csharp
[Table("vaccinations")]
public class Vaccination
{
    [Key]
    [Column("vaccination_id")]
    public int VaccinationId { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("manufacturer")]
    [MaxLength(100)]
    public string Manufacturer { get; set; }

    [Column("validity_period")]
    public int? ValidityPeriod { get; set; }

    // Навигационное свойство
    public virtual ICollection<AnimalVaccination> AnimalVaccinations { get; set; }
}
```

### 4.8 AnimalVaccination.cs

История вакцинаций (связь M:N между животными и вакцинами).

```csharp
[Table("animal_vaccinations")]
public class AnimalVaccination
{
    [Key]
    [Column("vaccination_id")]
    public int VaccinationId { get; set; }

    [Required]
    [Column("animal_id")]
    public int AnimalId { get; set; }

    [Required]
    [Column("vaccination_id")]
    public int VaccinationTypeId { get; set; }

    [Column("vaccination_date")]
    public DateTime VaccinationDate { get; set; }

    [Column("created_date")]
    public DateTime CreatedDate { get; set; }

    // Навигационные свойства
    [ForeignKey("AnimalId")]
    public virtual Animal Animal { get; set; }

    [ForeignKey("VaccinationTypeId")]
    public virtual Vaccination VaccinationType { get; set; }
}
```

---

## 5. Слой доступа к данным (Data)

### 5.1 AnimalShelterContext.cs

Главный класс контекста Entity Framework Core.

**Основные функции:**
1. Определение DbSet для всех таблиц
2. Настройка связей между таблицами
3. Конфигурация индексов и ограничений
4. Автоматическое преобразование DateTime в UTC

```csharp
public class AnimalShelterContext : DbContext
{
    public DbSet<Adopter> Adopters { get; set; }
    public DbSet<Animal> Animals { get; set; }
    public DbSet<AnimalStatus> AnimalStatuses { get; set; }
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Adoption> Adoptions { get; set; }
    public DbSet<AdoptionStatus> AdoptionStatuses { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<AnimalVaccination> AnimalVaccinations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка связей
        // Настройка индексов
        // Настройка конвертеров DateTime в UTC
    }
}
```

**Ключевые настройки:**

#### Связь 1:1 (Animal - MedicalRecord)
```csharp
modelBuilder.Entity<Animal>()
    .HasOne(a => a.MedicalRecord)
    .WithOne(m => m.Animal)
    .HasForeignKey<MedicalRecord>(m => m.AnimalId);
```

#### Связь 1:M (Adopter - Adoptions)
```csharp
modelBuilder.Entity<Adoption>()
    .HasOne(ad => ad.Adopter)
    .WithMany(a => a.Adoptions)
    .HasForeignKey(ad => ad.AdopterId)
    .OnDelete(DeleteBehavior.Restrict);
```

#### Автоматическое преобразование DateTime в UTC
```csharp
private void ConvertDatesToUtc()
{
    foreach (var entry in ChangeTracker.Entries())
    {
        foreach (var property in entry.Properties)
        {
            if (property.CurrentValue is DateTime dateTime 
                && dateTime.Kind != DateTimeKind.Utc)
            {
                property.CurrentValue = dateTime.ToUniversalTime();
            }
        }
    }
}
```

### 5.2 DbContextFactory.cs

Фабрика для создания экземпляров контекста базы данных.

```csharp
public class DbContextFactory
{
    public static AnimalShelterContext CreateDbContext(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AnimalShelterContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AnimalShelterContext(optionsBuilder.Options);
    }
}
```

**Использование:**
```csharp
string connectionString = "Host=localhost;Database=shelter;Username=postgres;Password=pass";
using var context = DbContextFactory.CreateDbContext(connectionString);
```

---

## 6. Репозитории (Repositories)

### 6.1 Repository.cs (Базовый репозиторий)

Реализует базовые CRUD операции для всех сущностей.

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AnimalShelterContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AnimalShelterContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual T GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate).ToList();
    }

    public virtual void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(int id)
    {
        var entity = GetById(id);
        if (entity != null)
        {
            Delete(entity);
        }
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public int SaveChanges()
    {
        return _context.SaveChanges();
    }
}
```

### 6.2 AnimalRepository.cs

Специализированный репозиторий для работы с животными.

```csharp
public class AnimalRepository : Repository<Animal>, IAnimalRepository
{
    public AnimalRepository(AnimalShelterContext context) : base(context)
    {
    }

    public IEnumerable<Animal> GetAnimalsByStatus(int statusId)
    {
        return _dbSet
            .Include(a => a.Status)
            .Where(a => a.StatusId == statusId)
            .OrderBy(a => a.DateAdmitted)
            .ToList();
    }

    public IEnumerable<Animal> GetAnimalsBySpecies(string species)
    {
        return _dbSet
            .Where(a => a.Species == species)
            .OrderBy(a => a.Name)
            .ToList();
    }

    public Animal GetAnimalWithMedicalRecord(int animalId)
    {
        return _dbSet
            .Include(a => a.MedicalRecord)
            .FirstOrDefault(a => a.AnimalId == animalId);
    }
}
```

**Дополнительные методы:**
- `GetAnimalsByStatus()` - получение животных по статусу
- `GetAnimalsBySpecies()` - получение животных по виду
- `GetAnimalWithMedicalRecord()` - получение животного с медкартой

### 6.3 AdopterRepository.cs

Репозиторий для работы с усыновителями.

```csharp
public class AdopterRepository : Repository<Adopter>, IAdopterRepository
{
    public AdopterRepository(AnimalShelterContext context) : base(context)
    {
    }

    public Adopter GetByEmail(string email)
    {
        return _dbSet.FirstOrDefault(a => a.Email == email);
    }
}
```

### 6.4 AdoptionRepository.cs

Репозиторий для работы с усыновлениями.

```csharp
public class AdoptionRepository : Repository<Adoption>, IAdoptionRepository
{
    public AdoptionRepository(AnimalShelterContext context) : base(context)
    {
    }

    public IEnumerable<Adoption> GetAdoptionsByStatus(int statusId)
    {
        return _dbSet
            .Include(a => a.Animal)
            .Include(a => a.Adopter)
            .Include(a => a.Status)
            .Where(a => a.StatusId == statusId)
            .OrderByDescending(a => a.AdoptionDate)
            .ToList();
    }

    public IEnumerable<Adoption> GetAdoptionsByDateRange(DateTime startDate, DateTime endDate)
    {
        return _dbSet
            .Include(a => a.Animal)
            .Include(a => a.Adopter)
            .Where(a => a.AdoptionDate >= startDate && a.AdoptionDate <= endDate)
            .OrderByDescending(a => a.AdoptionDate)
            .ToList();
    }

    public IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId)
    {
        return _dbSet
            .Include(a => a.Animal)
            .Where(a => a.AdopterId == adopterId)
            .OrderByDescending(a => a.AdoptionDate)
            .ToList();
    }
}
```

---

## 7. Бизнес-логика (Services)

### 7.1 AnimalService.cs

Сервис для управления животными.

**Основные методы:**

```csharp
public class AnimalService
{
    private readonly IAnimalRepository _animalRepository;

    // CREATE
    public void AddAnimal(string name, int age, string species, 
                         string breed, string gender, int statusId, 
                         string description)
    {
        var animal = new Animal
        {
            Name = name,
            Age = age,
            Species = species,
            Breed = breed,
            Gender = gender,
            StatusId = statusId,
            Description = description,
            DateAdmitted = DateTime.Now
        };

        _animalRepository.Add(animal);
        _animalRepository.SaveChanges();
    }

    // READ
    public IEnumerable<Animal> GetAllAnimals()
    {
        return _animalRepository.GetAll();
    }

    public Animal GetAnimalById(int id)
    {
        return _animalRepository.GetById(id);
    }

    // UPDATE
    public void UpdateAnimal(Animal animal)
    {
        _animalRepository.Update(animal);
        _animalRepository.SaveChanges();
    }

    // DELETE
    public void DeleteAnimal(int id)
    {
        _animalRepository.Delete(id);
        _animalRepository.SaveChanges();
    }

    // Дополнительные методы
    public IEnumerable<Animal> GetAnimalsByStatus(int statusId)
    {
        return _animalRepository.GetAnimalsByStatus(statusId);
    }

    public void UpdateAnimalStatus(int animalId, int newStatusId)
    {
        var animal = _animalRepository.GetById(animalId);
        if (animal != null)
        {
            animal.StatusId = newStatusId;
            _animalRepository.Update(animal);
            _animalRepository.SaveChanges();
        }
    }
}
```

### 7.2 AdopterService.cs

Сервис для управления усыновителями.

**Основные методы:**

```csharp
public class AdopterService
{
    private readonly IAdopterRepository _adopterRepository;

    public void AddAdopter(string firstName, string lastName, 
                          string email, string phone, string address)
    {
        // Проверка на дубликат email
        var existingAdopter = _adopterRepository.GetByEmail(email);
        if (existingAdopter != null)
        {
            throw new InvalidOperationException("Усыновитель с таким email уже существует");
        }

        var adopter = new Adopter
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Address = address,
            RegistrationDate = DateTime.Now
        };

        _adopterRepository.Add(adopter);
        _adopterRepository.SaveChanges();
    }

    public IEnumerable<Adopter> GetAllAdopters()
    {
        return _adopterRepository.GetAll();
    }

    public Adopter GetAdopterByEmail(string email)
    {
        return _adopterRepository.GetByEmail(email);
    }
}
```

### 7.3 AdoptionService.cs

Сервис для управления процессом усыновления.

**Основные методы:**

```csharp
public class AdoptionService
{
    private readonly IAdoptionRepository _adoptionRepository;
    private readonly IAnimalRepository _animalRepository;

    public void CreateAdoption(int adopterId, int animalId, string description)
    {
        // Проверка доступности животного
        var animal = _animalRepository.GetById(animalId);
        if (animal == null)
        {
            throw new InvalidOperationException("Животное не найдено");
        }

        if (animal.StatusId == 2) // Adopted
        {
            throw new InvalidOperationException("Животное уже усыновлено");
        }

        var adoption = new Adoption
        {
            AdopterId = adopterId,
            AnimalId = animalId,
            AdoptionDate = DateTime.Now,
            StatusId = 1, // Pending
            Description = description
        };

        _adoptionRepository.Add(adoption);
        _adoptionRepository.SaveChanges();
    }

    public void ApproveAdoption(int adoptionId)
    {
        var adoption = _adoptionRepository.GetById(adoptionId);
        if (adoption == null)
        {
            throw new InvalidOperationException("Заявка не найдена");
        }

        adoption.StatusId = 2; // Approved
        _adoptionRepository.Update(adoption);

        // Обновление статуса животного
        var animal = _animalRepository.GetById(adoption.AnimalId);
        if (animal != null)
        {
            animal.StatusId = 2; // Adopted
            _animalRepository.Update(animal);
        }

        _adoptionRepository.SaveChanges();
    }

    public void RejectAdoption(int adoptionId, string reason)
    {
        var adoption = _adoptionRepository.GetById(adoptionId);
        if (adoption == null)
        {
            throw new InvalidOperationException("Заявка не найдена");
        }

        adoption.StatusId = 3; // Rejected
        adoption.Description = reason;
        _adoptionRepository.Update(adoption);
        _adoptionRepository.SaveChanges();
    }
}
```

### 7.4 ReportService.cs

Сервис для генерации отчетов и аналитики.

**Основные методы:**

```csharp
public class ReportService
{
    private readonly AnimalShelterContext _context;

    // Выборка 1: Животные по статусу с сортировкой
    public List<Animal> GetAnimalsByStatusReport(int statusId)
    {
        return _context.Animals
            .Include(a => a.Status)
            .Where(a => a.StatusId == statusId)
            .OrderBy(a => a.DateAdmitted)
            .ToList();
    }

    // Выборка 2: Усыновления по датам с фильтрацией
    public List<Adoption> GetAdoptionsByDateRangeReport(
        DateTime startDate, DateTime endDate, int statusId)
    {
        return _context.Adoptions
            .Include(a => a.Animal)
            .Include(a => a.Adopter)
            .Include(a => a.Status)
            .Where(a => a.AdoptionDate >= startDate && 
                       a.AdoptionDate <= endDate && 
                       a.StatusId == statusId)
            .OrderByDescending(a => a.AdoptionDate)
            .ToList();
    }

    // Выборка 3: Животные по виду и возрасту
    public List<Animal> GetAnimalsBySpeciesAndAgeReport(
        string species, int minAge, int maxAge)
    {
        return _context.Animals
            .Include(a => a.MedicalRecord)
            .Include(a => a.Status)
            .Where(a => a.Species == species && 
                       a.Age >= minAge && 
                       a.Age <= maxAge)
            .OrderBy(a => a.Age)
            .ThenBy(a => a.Name)
            .ToList();
    }

    // Статистика по приюту
    public void DisplayShelterStatistics()
    {
        var totalAnimals = _context.Animals.Count();
        var inShelter = _context.Animals.Count(a => a.StatusId == 1);
        var adopted = _context.Animals.Count(a => a.StatusId == 2);
        var inTreatment = _context.Animals.Count(a => a.StatusId == 3);

        Console.WriteLine("\n=== СТАТИСТИКА ПРИЮТА ===");
        Console.WriteLine($"Всего животных: {totalAnimals}");
        Console.WriteLine($"В приюте: {inShelter}");
        Console.WriteLine($"Усыновлено: {adopted}");
        Console.WriteLine($"На лечении: {inTreatment}");
    }
}
```

---

## 8. Интерфейсы (Interfaces)

### 8.1 IRepository.cs

Базовый интерфейс для всех репозиториев.

```csharp
public interface IRepository<T> where T : class
{
    T GetById(int id);
    IEnumerable<T> GetAll();
    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(int id);
    void Delete(T entity);
    int SaveChanges();
}
```

### 8.2 IAnimalRepository.cs

```csharp
public interface IAnimalRepository : IRepository<Animal>
{
    IEnumerable<Animal> GetAnimalsByStatus(int statusId);
    IEnumerable<Animal> GetAnimalsBySpecies(string species);
    Animal GetAnimalWithMedicalRecord(int animalId);
}
```

### 8.3 IAdopterRepository.cs

```csharp
public interface IAdopterRepository : IRepository<Adopter>
{
    Adopter GetByEmail(string email);
}
```

### 8.4 IAdoptionRepository.cs

```csharp
public interface IAdoptionRepository : IRepository<Adoption>
{
    IEnumerable<Adoption> GetAdoptionsByStatus(int statusId);
    IEnumerable<Adoption> GetAdoptionsByDateRange(DateTime startDate, DateTime endDate);
    IEnumerable<Adoption> GetAdoptionsByAdopter(int adopterId);
}
```

---

## 9. Вспомогательные классы (Helpers)

### 9.1 ConsoleHelper.cs

Утилиты для упрощения работы с консольным интерфейсом.

**Основные методы:**

```csharp
public static class ConsoleHelper
{
    // Вывод заголовка
    public static void PrintHeader(string title)
    {
        Console.Clear();
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine($" {title}");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine();
    }

    // Вывод ошибки
    public static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ОШИБКА] {message}");
        Console.ResetColor();
    }

    // Вывод успеха
    public static void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[УСПЕХ] {message}");
        Console.ResetColor();
    }

    // Вывод предупреждения
    public static void PrintWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[ВНИМАНИЕ] {message}");
        Console.ResetColor();
    }

    // Пауза
    public static void PressAnyKeyToContinue()
    {
        Console.WriteLine("\nНажмите любую клавишу для продолжения...");
        Console.ReadKey();
    }

    // Чтение строки
    public static string ReadLine(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine() ?? string.Empty;
    }

    // Чтение числа
    public static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                return result;
            }
            PrintError("Введите корректное число!");
        }
    }

    // Чтение даты
    public static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (DateTime.TryParse(Console.ReadLine(), out DateTime result))
            {
                return result;
            }
            PrintError("Введите корректную дату (формат: дд.мм.гггг)!");
        }
    }
}
```

---

## 10. Главное приложение (Program.cs)

### 10.1 Структура главного меню

```
=== СИСТЕМА УПРАВЛЕНИЯ ПРИЮТОМ ДЛЯ ЖИВОТНЫХ ===
1. Управление животными
2. Управление усыновителями
3. Управление усыновлениями
4. Отчеты и выборки
0. Выход
```

### 10.2 Подменю "Управление животными"

```
1. Просмотр всех животных
2. Добавить животное
3. Редактировать животное
4. Удалить животное
5. Поиск по статусу
0. Назад
```

### 10.3 Подменю "Управление усыновителями"

```
1. Просмотр всех усыновителей
2. Добавить усыновителя
3. Редактировать усыновителя
4. Удалить усыновителя
0. Назад
```

### 10.4 Подменю "Управление усыновлениями"

```
1. Просмотр всех заявок
2. Создать заявку на усыновление
3. Одобрить заявку
4. Отклонить заявку
5. Поиск по статусу
0. Назад
```

### 10.5 Подменю "Отчеты и выборки"

```
1. Животные по статусу
2. Усыновления за период
3. Животные по виду и возрасту
4. Статистика приюта
0. Назад
```

### 10.6 Инициализация приложения

```csharp
static void Main(string[] args)
{
    // Включение legacy режима для DateTime
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    // Строка подключения
    string connectionString = "Host=localhost;Port=5432;Database=animal_shelter;Username=postgres;Password=your_password";

    // Создание контекста
    using var context = DbContextFactory.CreateDbContext(connectionString);

    // Инициализация репозиториев
    IAnimalRepository animalRepo = new AnimalRepository(context);
    IAdopterRepository adopterRepo = new AdopterRepository(context);
    IAdoptionRepository adoptionRepo = new AdoptionRepository(context);

    // Инициализация сервисов
    var animalService = new AnimalService(animalRepo);
    var adopterService = new AdopterService(adopterRepo);
    var adoptionService = new AdoptionService(adoptionRepo, animalRepo);
    var reportService = new ReportService(context);

    // Главный цикл приложения
    bool exit = false;
    while (!exit)
    {
        // Отображение главного меню
        // Обработка выбора пользователя
    }
}
```

---

## 11. Нормализация базы данных

### Итоговый анализ нормальных форм

#### 1NF (Первая нормальная форма)

✅ **Выполнено:**

- Поле `full_name` разбито на `first_name` и `last_name` в таблице `adopters`
- Все поля содержат атомарные (неделимые) значения
- Каждая таблица имеет первичный ключ
- Отсутствуют повторяющиеся группы данных

**Пример:**
```sql
-- Было (неправильно):
full_name VARCHAR(200)  -- "Иван Петров"

-- Стало (правильно):
first_name VARCHAR(100)  -- "Иван"
last_name VARCHAR(100)   -- "Петров"
```

#### 2NF (Вторая нормальная форма)

✅ **Выполнено:**

- Все таблицы имеют простые (не составные) первичные ключи
- Нет частичных зависимостей от ключа
- Каждое неключевое поле функционально зависит от всего первичного ключа

**Примеры:**
- В таблице `animals` все поля зависят только от `animal_id`
- В таблице `adoptions` все поля зависят только от `adoption_id`
- Все справочники (`animal_status`, `adoption_status`, `vaccinations`) имеют простые ключи

#### 3NF (Третья нормальная форма)

✅ **Выполнено:**

- Добавлено поле `owner_id` в таблицу `medical_records` для устранения транзитивных зависимостей
- Отсутствуют транзитивные зависимости между неключевыми полями
- Все неключевые атрибуты зависят только от первичного ключа

**Устранение транзитивных зависимостей:**

```sql
-- Было (транзитивная зависимость):
medical_records:
  record_id (PK)
  animal_id (FK)
  -- Владелец определялся через animal_id → adopter_id

-- Стало (прямая зависимость):
medical_records:
  record_id (PK)
  animal_id (FK)
  owner_id (FK)  -- Прямая связь с владельцем
```

**Вынесение справочников:**

1. **animal_status** - статусы животных вынесены в отдельную таблицу
2. **adoption_status** - статусы усыновлений вынесены в отдельную таблицу
3. **vaccinations** - справочник вакцин отделён от истории вакцинаций

**Результат:**
- Отсутствует дублирование данных
- Обеспечена целостность данных через внешние ключи
- Упрощено обновление справочной информации
- Минимизированы аномалии вставки, обновления и удаления

### Примеры соответствия 3NF

#### Таблица animals

```sql
animal_id → name, age, species, breed, gender, date_admitted, status_id, description
```
Все атрибуты зависят только от `animal_id`, транзитивных зависимостей нет.

#### Таблица medical_records

```sql
record_id → animal_id, owner_id, blood_type, allergies, chronic_conditions, 
            special_needs, veterinarian_notes, last_checkup_date, created_date
```
Поле `owner_id` устраняет транзитивную зависимость через `animal_id`.

#### Таблица adoptions

```sql
adoption_id → adopter_id, animal_id, adoption_date, status_id, return_date, description
```
Все поля зависят напрямую от `adoption_id`.

---

## 12. Установка и запуск

### 12.1 Предварительные требования

- .NET 9.0 SDK
- PostgreSQL 12+
- Visual Studio Code или Visual Studio 2022

### 12.2 Установка

1. **Клонируйте репозиторий**
   ```bash
   git clone https://github.com/yourusername/animal-shelter.git
   cd animal-shelter
   ```

2. **Создайте базу данных**
   ```bash
   psql -U postgres
   CREATE DATABASE animal_shelter;
   \q
   ```

3. **Выполните SQL-скрипт для создания таблиц**
   ```bash
   psql -U postgres -d animal_shelter -f database/schema.sql
   ```

4. **Настройте строку подключения**

   Отредактируйте `Program.cs`:
   ```csharp
   string connectionString = "Host=localhost;Port=5432;Database=animal_shelter;Username=postgres;Password=your_password";
   ```

5. **Установите зависимости**
   ```bash
   dotnet restore
   ```

6. **Соберите проект**
   ```bash
   dotnet build
   ```

### 12.3 Запуск

```bash
dotnet run
```

### 12.4 Тестирование

#### Сценарий 1: Регистрация животного и создание медкарты

1. Запустите приложение
2. Выберите "1. Управление животными"
3. Выберите "2. Добавить животное"
4. Заполните данные о животном
5. Создайте медицинскую карту для животного

#### Сценарий 2: Процесс усыновления

1. Зарегистрируйте усыновителя (Меню 2 → Пункт 2)
2. Создайте заявку на усыновление (Меню 3 → Пункт 2)
3. Одобрите заявку (Меню 3 → Пункт 3)
4. Проверьте изменение статуса животного

#### Сценарий 3: Отчёты и аналитика

1. Откройте меню отчетов (Меню 4)
2. Выберите "1. Животные по статусу"
3. Введите статус "InShelter"
4. Просмотрите список с сортировкой по дате поступления

### 12.5 Устранение типичных проблем

#### Ошибка подключения к БД

```
Npgsql.NpgsqlException: Connection refused
```

**Решение:**
- Проверьте, что PostgreSQL запущен
- Убедитесь в правильности строки подключения
- Проверьте настройки фаервола

#### Ошибка DateTime Kind=Local

```
Cannot write DateTime with Kind=Local to PostgreSQL
```

**Решение:** Убедитесь, что в `Program.cs` есть строка:
```csharp
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```

#### Ошибка версий пакетов

```
NU1605: Detected package downgrade
```

**Решение:**
```bash
dotnet clean
dotnet nuget locals all --clear
dotnet restore
```

---

## Заключение

Данная документация описывает полную структуру проекта **Animal Shelter Management System**, включая архитектуру базы данных, реализацию паттернов проектирования, бизнес-логику и примеры использования.

Проект демонстрирует:
- ✅ Правильную нормализацию БД до 3NF
- ✅ Использование паттерна Repository
- ✅ Разделение на слои (Models, Data, Services)
- ✅ CRUD операции и сложные выборки
- ✅ Работу с Entity Framework Core и PostgreSQL

---

**Автор:** Ваше Имя  
**Дата:** 10 октября 2025  
**Версия:** 1.0
