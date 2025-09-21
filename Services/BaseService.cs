using HospitalApp.DBContextHospital;

namespace HospitalApp.Services;

public abstract class BaseService
{
    protected HospitalDbContext _context;

    public BaseService(HospitalDbContext context)
    {
        _context = context;
    }

}
