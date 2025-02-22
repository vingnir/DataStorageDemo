### **Understanding DTOs, Entities, Repositories, and Services in a Clean Architecture**
 
---

## **1️⃣ DTO (Data Transfer Object)**
### **What is a DTO?**
- A **DTO** is an **intermediary object** that is used **to transfer data** between layers (e.g., from the database to the API).
- It ensures **separation of concerns** by **hiding unnecessary database details** from the API response.

### **Why Use DTOs?**
✅ Improves **security** by exposing only necessary fields.  
✅ Avoids **over-fetching** (sending too much data).  
✅ Ensures **loose coupling** between layers.

### **Example**
#### **Entity (Database Model)**
```csharp
public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}
```
#### **DTO (API Model)**
```csharp
public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```
Here, the DTO **hides** `Description` and `CreatedAt` from the API response.

---

## **2️⃣ Entities**
### **What is an Entity?**
- **Entities** represent **database tables** and are used by **EF Core** to map objects to the database.
- Stored inside `Data/Entities/`.

### **Example**
```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```
- This maps to a **Customers** table in the database.

---

## **3️⃣ Repositories**
### **What is a Repository?**
- **Repositories** act as a **data access layer**, encapsulating **database logic**.
- Used to **fetch, insert, update, and delete** data without exposing EF Core logic to services.

### **Why Use Repositories?**
✅ **Decouples** database logic from the service layer.  
✅ Prevents **direct EF Core calls** in services.  
✅ Centralizes **data access logic**.

---

### **Base Repository (Generic CRUD Operations)**
```csharp
public class BaseRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    public BaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

---

### **Specific Repository (Extending BaseRepository)**
```csharp
public class ProjectRepository : BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(AppDbContext context) : base(context) { }

    public async Task<List<Project>> GetProjectsByName(string name)
    {
        return await _context.Projects.Where(p => p.Name.Contains(name)).ToListAsync();
    }
}
```

---

## **4️⃣ Services**
### **What is a Service?**
- **Services** handle **business logic** (e.g., calculations, transformations, etc.).
- They call **repositories** to retrieve data but **never interact with EF Core directly**.
- Located in `Business/Services/`.

### **Example**
```csharp
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllProjects()
    {
        var projects = await _projectRepository.GetAllAsync();
        return projects.Select(p => new ProjectDto { Id = p.Id, Name = p.Name });
    }
}
```

---

## **5️⃣ API Controllers**
### **What is a Controller?**
- The **controller** handles HTTP requests and responses.
- Calls the **service layer**, never directly interacts with the database.

### **Example**
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _projectService.GetAllProjects();
        return Ok(projects);
    }
}
```

---

## **Summary of Flow**
1️⃣ **Client calls API** → (`GET /api/projects`)  
2️⃣ **Controller calls Service** → (`ProjectService`)  
3️⃣ **Service calls Repository** → (`ProjectRepository`)  
4️⃣ **Repository calls Database** → (`AppDbContext` with EF Core)  
5️⃣ **Data is returned** → **Repository → Service → Controller → API Response**

---

## **Final Takeaways**
| Layer | Responsibility | Example Files |
|--------|-----------------|--------------|
| **DTOs** | Transfer only required data | `ProjectDto.cs` |
| **Entities** | Represent database tables | `Project.cs` |
| **Repositories** | Handle database operations | `ProjectRepository.cs` |
| **Services** | Implement business logic | `ProjectService.cs` |
| **Controllers** | Handle API requests | `ProjectsController.cs` |

---
### **🔎 Best Practices**
✅ **Always use DTOs** to avoid exposing database structure.  
✅ **Keep controllers thin** → Move logic to **Services**.  
✅ **Repositories** should only **handle data access**, nothing more.  
✅ **Services** should never interact with EF Core directly.  
✅ Use **Dependency Injection** for services and repositories.

---