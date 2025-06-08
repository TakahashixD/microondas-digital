namespace backend_microondas.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgramaAquecimentoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 50),
                        Alimento = c.String(nullable: false, maxLength: 100),
                        Tempo = c.Int(nullable: false),
                        Potencia = c.Int(nullable: false),
                        CaractereAquecimento = c.String(nullable: false, maxLength: 1),
                        Instrucoes = c.String(nullable: false, maxLength: 500),
                        Customizado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true)
                .Index(t => t.CaractereAquecimento, unique: true);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 100),
                        Senha = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Nome, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Usuarios", new[] { "Nome" });
            DropIndex("dbo.ProgramaAquecimentoes", new[] { "CaractereAquecimento" });
            DropIndex("dbo.ProgramaAquecimentoes", new[] { "Nome" });
            DropTable("dbo.Usuarios");
            DropTable("dbo.ProgramaAquecimentoes");
        }
    }
}
