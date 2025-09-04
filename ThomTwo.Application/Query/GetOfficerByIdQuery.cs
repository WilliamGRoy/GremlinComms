using MediatR;
using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;

namespace ThomTwo.Application.Features.Officers.Queries
{
    public class GetOfficerByIdQuery : IRequest<Officer>
    {
        public string Id { get; set; }
    }

    public class GetOfficerByIdQueryHandler : IRequestHandler<GetOfficerByIdQuery, Officer>
    {
        private readonly IOfficerRepository _officerRepository;

        public GetOfficerByIdQueryHandler(IOfficerRepository officerRepository)
        {
            _officerRepository = officerRepository;
        }

        public Task<Officer> Handle(GetOfficerByIdQuery request, CancellationToken cancellationToken)
        {
            return _officerRepository.GetByIdAsync(request.Id);
        }
    }
}