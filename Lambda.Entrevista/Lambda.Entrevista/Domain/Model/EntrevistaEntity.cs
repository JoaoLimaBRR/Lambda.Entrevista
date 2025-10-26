using Amazon.DynamoDBv2.DataModel;

namespace Lambda.Entrevista.Domain.Model
{
    [DynamoDBTable("TABELA_PLACEHOLDER")]
    public class EntrevistaEntity
    {
        [DynamoDBHashKey]
        public string Id { get; set; }


        [DynamoDBProperty]
        public string Nome { get; set; }


        [DynamoDBProperty]
        public string Email { get; set; }


        [DynamoDBProperty]
        public string Cargo { get; set; }


        [DynamoDBProperty]
        public string Status { get; set; }


        [DynamoDBProperty]
        public DateTime CriadoEm { get; set; }

        [DynamoDBProperty]
        public string TokenJwtBase64 { get; set; }
    }
}
