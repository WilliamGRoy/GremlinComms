using MediatR;
using ThomTwo.Domain.Entities;
using ThomTwo.Domain.Repository;

namespace ThomTwo.Application.Features.Officers.Commands
{
    public class CreateOfficerCommand : IRequest<Officer>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class CreateOfficerCommandHandler : IRequestHandler<CreateOfficerCommand, Officer>
    {
        private readonly IOfficerRepository _officerRepository;

        public CreateOfficerCommandHandler(IOfficerRepository officerRepository)
        {
            _officerRepository = officerRepository;
        }

        public async Task<Officer> Handle(CreateOfficerCommand request, CancellationToken cancellationToken)
        {
            var officer = new Officer
            {
                Id = request.Id, Name = request.Name, Age = request.Age
            };

            await _officerRepository.AddAsync(officer);
            return officer;
        }
    }
}