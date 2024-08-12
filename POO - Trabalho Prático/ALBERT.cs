using System;
using System.Collections.Generic;
using System.IO;

class Conta
{
    public int LeituraMesAnterior { get; set; }
    public int LeituraMesAtual { get; set; }

    public Conta(int leituraMesAnterior, int leituraMesAtual)
    {
        LeituraMesAnterior = leituraMesAnterior;
        LeituraMesAtual = leituraMesAtual;
    }

    // Método para calcular o consumo
    public double CalcularConsumo()
    {
        return LeituraMesAtual - LeituraMesAnterior;    
    }
}

// Classe para representar uma conta de energia
class ContaEnergia : Conta
{
    public string TipoConsumidor { get; set; }

    public ContaEnergia(int leituraMesAnterior, int leituraMesAtual, string tipoConsumidor)
        : base(leituraMesAnterior, leituraMesAtual)
    {
        TipoConsumidor = tipoConsumidor;
    }

    public double CalcularValorConta()
    {
        double tarifa = (TipoConsumidor == "residencial") ? 0.46 : 0.41;
        double consumo = CalcularConsumo();
        double valorConsumo = consumo * tarifa;

        // Adiciona a contribuição para iluminação pública
        double contribuicaoIluminacaoPublica = 13.25;
        valorConsumo += contribuicaoIluminacaoPublica;

        // Calcula o imposto com base no tipo de consumidor
        double imposto = (TipoConsumidor == "residencial") ? 0.4285 : 0.2195;

        // Verifica a isenção do imposto para consumidores residenciais com consumo abaixo de 90KW/h
        if (TipoConsumidor == "residencial" && consumo < 90)
        {
            imposto = 0.0;
        }

        double valorImposto = valorConsumo * imposto;

        // Calcula o valor total da conta
        double valorTotalConta = valorConsumo + valorImposto;

        return valorTotalConta;
    }
}

// Classe para representar uma conta de água
class ContaAgua : Conta
{
    public string TipoConsumidor { get; set; }

    public ContaAgua(int leituraMesAnterior, int leituraMesAtual, string tipoConsumidor)
        : base(leituraMesAnterior, leituraMesAtual)
    {
        TipoConsumidor = tipoConsumidor;
    }

    public double CalcularValorConta()
    {
        double[] tarifasAgua = GetTarifasAgua();
        double[] tarifasEsgoto = GetTarifasEsgoto();
        double consumo = CalcularConsumo();
        double valorContaAgua = 0;
        double valorContaEsgoto = 0;

        // Calcula o valor da conta de água
        foreach (var tarifa in tarifasAgua)
        {
            if (consumo <= 0)
                break;
            if (consumo < 5)
                valorContaAgua += consumo * tarifa;
            else
                valorContaAgua += 5 * tarifa;
            consumo -= 5;
        }

        // Calcular o valor da conta de esgoto
        consumo = CalcularConsumo(); // Reinicia o consumo para o cálculo de esgoto
        foreach (var tarifa in tarifasEsgoto)
        {
            if (consumo <= 0)
                break;
            if (consumo < 5)
                valorContaEsgoto += consumo * tarifa;
            else
                valorContaEsgoto += 5 * tarifa;
            consumo -= 5;
        }

        // Calcula o valor total da conta de água e esgoto
        double valorTotalConta = valorContaAgua + valorContaEsgoto;

        // Adiciona 3% de COFINS
        valorTotalConta *= 1.03;

        return valorTotalConta;
    }


    private double[] GetTarifasAgua()
    {
        if (TipoConsumidor == "residencial social")
        {
            return new double[] { 10.08, 2.241, 5.447, 5.461, 5.487, 10.066 };
        }
        else if (TipoConsumidor == "residencial")
        {
            return new double[] { 2.241, 5.447, 5.461, 5.487, 10.066 };
        }
        else if (TipoConsumidor == "comercial")
        {
            return new double[] { 25.79, 4.299, 8.221, 8.288, 8.329 };
        }
        return new double[] { 0.0 };
    }

    private double[] GetTarifasEsgoto()
    {
        if (TipoConsumidor == "residencial social")
        {
            return new double[] { 5.05, 1.122, 2.724, 2.731, 2.744, 5.035 };
        }
        else if (TipoConsumidor == "residencial")
        {
            return new double[] { 1.122, 2.724, 2.731, 2.744, 5.035 };
        }
        else if (TipoConsumidor == "comercial")
        {
            return new double[] { 12.90, 2.149, 4.111, 4.144, 4.165 };
        }
        return new double[] { 0.0 };
    }
}


class Cliente
{
    public string ID { get; set; }
    public List<Conta> Contas { get; set; }

    public Cliente(string id)
    {
        ID = id;
        Contas = new List<Conta>();
    }
}
class Program
{
    private static Dictionary<string, Cliente> clientes = new Dictionary<string, Cliente>();

    static void Main()
    {

        LerInformacoesDeArquivo("dados_contas.txt");
        GravarTodasContasEmArquivo("contas_saida");
        

        while (true)
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1. Calcular consumo de energia/água no último mês");
            Console.WriteLine("2. Calcular valor total da conta");
            Console.WriteLine("3. Calcular valor da conta sem impostos");
            Console.WriteLine("4. Calcular variação da conta entre dois meses");
            Console.WriteLine("5. Calcular valor médio da conta de energia/água");
            Console.WriteLine("6. Encontrar mês com conta de maior valor");
            Console.WriteLine("7. Cadastrar um novo Cliente ");
            Console.WriteLine("8. Sair");

            string escolha = Console.ReadLine();

            switch (escolha)
            {
                case "1":
                    CalcularConsumoUltimoMes();
                    break;
                case "2":
                    CalcularValorTotalConta();
                    break;
                case "3":
                    CalcularValorContaSemImpostos();
                    break;
                case "4":
                    CalcularVariacaoConta();
                    break;
                case "5":
                    CalcularValorMedioConta();
                    break;
                case "6":
                    EncontrarMaiorValorConta();
                    break;
                case "7":
                    CadastrarCliente.RegistrarCliente(); // Passando a referência da variável clientes
                    break;
                case "8":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Opção inválida. Por favor, escolha uma opção válida.");
                    break;
            }
        }
    }

    static void LerInformacoesDeArquivo(string nomeArquivo)
    {
        try
        {
            string[] linhas = File.ReadAllLines(nomeArquivo);

            // Processa as linhas e cria objetos de cliente e suas contass
            foreach (string linha in linhas)
            {
                string[] partes = linha.Split(',');

                if (partes.Length < 5)
                {
                    Console.WriteLine("Dados inválidos na linha: " + linha);
                    continue;
                }

                string identificadorCliente = partes[0];
                string tipoConta = partes[1];
                int leituraMesAnterior = int.Parse(partes[2]);
                int leituraMesAtual = int.Parse(partes[3]);
                string tipoConsumidor = partes[4];

                if (!clientes.ContainsKey(identificadorCliente))
                {
                    clientes[identificadorCliente] = new Cliente(identificadorCliente);
                }

                Conta novaConta;

                if (tipoConta == "ContaEnergia")
                {
                    novaConta = new ContaEnergia(leituraMesAnterior, leituraMesAtual, tipoConsumidor);
                }
                else if (tipoConta == "ContaAgua")
                {
                    novaConta = new ContaAgua(leituraMesAnterior, leituraMesAtual, tipoConsumidor);
                }
                else
                {
                    Console.WriteLine("Tipo de conta desconhecido na linha: " + linha);
                    continue;
                }

                clientes[identificadorCliente].Contas.Add(novaConta);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ocorreu um erro ao ler o arquivo: " + ex.Message);
        }

        catch (UnauthorizedAccessException ex)
        {
        Console.WriteLine("Sem permissão para acessar o arquivo: " + ex.Message);
        }
        catch (Exception ex)
        {
        Console.WriteLine("Ocorreu um erro inesperado: " + ex.Message);
        }


    }

    class CadastrarCliente
    {
        public static void RegistrarCliente()
        {
            Console.WriteLine("Registro de Novo Cliente");

            Console.Write("Identificador do Cliente: ");
            string identificadorCliente = Console.ReadLine();

            if (clientes.ContainsKey(identificadorCliente))
            {
                Console.WriteLine("Este cliente já existe. Deseja adicionar contas a este cliente? (S/N)");
                string resposta = Console.ReadLine();

                if (resposta.ToLower() == "n")
                {
                    return;
                }
            }
            else
            {
                clientes[identificadorCliente] = new Cliente(identificadorCliente);
            }

            Console.Write("Tipo da Conta (ContaEnergia ou ContaAgua): ");
            string tipoDaConta = Console.ReadLine();  // Alterado o nome do parâmetro

            if (tipoDaConta != "ContaEnergia" && tipoDaConta != "ContaAgua")
            {
                Console.WriteLine("Tipo de conta inválido. Use 'ContaEnergia' ou 'ContaAgua'.");
                return;
            }

            Console.Write("Leitura Mês Anterior: ");
            if (!int.TryParse(Console.ReadLine(), out int leituraMesAnterior))
            {
                Console.WriteLine("Leitura Mês Anterior inválida. Deve ser um número inteiro.");
                return;
            }

            Console.Write("Leitura Mês Atual: ");
            if (!int.TryParse(Console.ReadLine(), out int leituraMesAtual))
            {
                Console.WriteLine("Leitura Mês Atual inválida. Deve ser um número inteiro.");
                return;
            }

            Console.Write("Tipo de Consumidor: ");
            string tipoConsumidor = Console.ReadLine();

            if (tipoDaConta == "ContaEnergia")
            {
                ContaEnergia novaContaEnergia = new ContaEnergia(leituraMesAnterior, leituraMesAtual, tipoConsumidor);
                clientes[identificadorCliente].Contas.Add(novaContaEnergia);
            }
            else if (tipoDaConta == "ContaAgua")
            {
                ContaAgua novaContaAgua = new ContaAgua(leituraMesAnterior, leituraMesAtual, tipoConsumidor);
                clientes[identificadorCliente].Contas.Add(novaContaAgua);
            }

            Console.WriteLine("Cliente e conta registrados com sucesso.");

            try
            {
                using (StreamWriter sw = new StreamWriter("dados_contas.txt", true))
                {
                    sw.WriteLine($"{identificadorCliente},{tipoDaConta},{leituraMesAnterior},{leituraMesAtual},{tipoConsumidor}");
                }
            }
            catch (IOException ex)
            {
             Console.WriteLine("Ocorreu um erro ao gravar no arquivo: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
            Console.WriteLine("Sem permissão para escrever no arquivo: " + ex.Message);
            }
            catch (Exception ex)
            {
            Console.WriteLine("Ocorreu um erro inesperado: " + ex.Message);
            }
        }
    }

    public static void GravarTodasContasEmArquivo(string nomeArquivo)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(nomeArquivo))
            {
                sw.WriteLine("Informações de Todas as Contas:");

                foreach (var cliente in clientes.Values)
                {
                    sw.WriteLine($"Cliente: {cliente.ID}");
                    foreach (var conta in cliente.Contas)
                    {
                        if (conta is ContaEnergia contaEnergia)
                        {
                            sw.WriteLine("Tipo de Conta: Energia");
                            sw.WriteLine($"Leitura Mês Anterior: {contaEnergia.LeituraMesAnterior}");
                            sw.WriteLine($"Leitura Mês Atual: {contaEnergia.LeituraMesAtual}");
                            sw.WriteLine($"Tipo Consumidor: {contaEnergia.TipoConsumidor}");
                            sw.WriteLine($"Valor da Conta: R$ {contaEnergia.CalcularValorConta():F2}");
                        }
                        else if (conta is ContaAgua contaAgua)
                        {
                            sw.WriteLine("Tipo de Conta: Água");
                            sw.WriteLine($"Leitura Mês Anterior: {contaAgua.LeituraMesAnterior}");
                            sw.WriteLine($"Leitura Mês Atual: {contaAgua.LeituraMesAtual}");
                            sw.WriteLine($"Tipo Consumidor: {contaAgua.TipoConsumidor}");
                            sw.WriteLine($"Valor da Conta: R$ {contaAgua.CalcularValorConta():F2}");
                        }

                        sw.WriteLine();
                    }
                }

                Console.WriteLine("Informações de todas as contas registradas em 'contas_saida.txt'.");
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ocorreu um erro ao gravar o arquivo: " + ex.Message);
        }
    }

    static void CalcularConsumoUltimoMes()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 0)
            {
                // Encontra a conta mais recente (com a leitura do mês atual mais alta)
                var contaMaisRecente = contas.OfType<Conta>().OrderByDescending(c => c.LeituraMesAtual).First();

                // Calcula o consumo do último mês
                double consumoUltimoMes = contaMaisRecente.CalcularConsumo();


                if (tipoDaConta == "energia")
                {
                    Console.WriteLine($"Consumo de {tipoDaConta} do último mês: {consumoUltimoMes} KW/h");
                }
                else if (tipoDaConta == "agua")
                {
                    Console.WriteLine($"Consumo de {tipoDaConta} do último mês: {consumoUltimoMes} m³");
                }

                // registra as informações no arquivo "resultados.txt"
                using (StreamWriter sw = new StreamWriter("resultados.txt", true))
                {
                    sw.WriteLine($"Cliente: {cliente.ID}, Tipo de Consumidor: {tipoDaConta}, Consumo do Último Mês: {consumoUltimoMes}");
                }

                Console.WriteLine("Informações registradas em 'resultados.txt'.");
                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui contas do tipo {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }

    static void CalcularValorTotalConta()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 0)
            {
                double valorTotalConta = 0.0;

                // Cálculo do valor total por iteração
                foreach (var conta in contas)
                {
                    // Certifica-se de que a conta seja do tipo correto antes de chamar o método
                    if (tipoDaConta == "energia" && conta is ContaEnergia contaEnergia)
                    {
                        valorTotalConta += contaEnergia.CalcularValorConta();
                    }
                    else if (tipoDaConta == "agua" && conta is ContaAgua contaAgua)
                    {
                        valorTotalConta += contaAgua.CalcularValorConta();
                    }
                }

                Console.WriteLine($"Valor total da conta de {tipoDaConta}: R$ {valorTotalConta:F2}");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui contas do tipo {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }

    static void CalcularValorContaSemImpostos()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 0)
            {
                double valorTotalContaSemImpostos = 0.0;

                // Cálculo do valor total sem impostos por iteração
                foreach (var conta in contas)
                {
                    // Certifica-se de que a conta seja do tipo correto antes de chamar o método
                    if (tipoDaConta == "energia" && conta is ContaEnergia contaEnergia)
                    {
                        valorTotalContaSemImpostos += contaEnergia.CalcularConsumo() * 0.46; // Considerando apenas o valor do consumo sem impostos
                    }
                    else if (tipoDaConta == "agua" && conta is ContaAgua contaAgua)
                    {
                        valorTotalContaSemImpostos += contaAgua.CalcularValorConta() / 1.03; // Considerando apenas o valor da conta de água sem impostos
                    }
                }

                Console.WriteLine($"Valor total da conta de {tipoDaConta} sem impostos: R$ {valorTotalContaSemImpostos:F2}");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui contas do tipo {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }

    static void CalcularVariacaoConta()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 1)
            {
                // Ordena as contas por leituraMesAtual para garantir a ordem correta
                contas.Sort((c1, c2) => c1.LeituraMesAtual.CompareTo(c2.LeituraMesAtual));

                // Calcula a variação entre a conta mais antiga e a mais recente
                int leituraMesAnteriorMaisAntiga = contas.First().LeituraMesAnterior;
                int leituraMesAtualMaisRecente = contas.Last().LeituraMesAtual;

                int variacao = leituraMesAtualMaisRecente - leituraMesAnteriorMaisAntiga;

                Console.WriteLine($"Variação na leitura da conta de {tipoDaConta}: {variacao}");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui histórico suficiente para calcular a variação da conta de {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }

    static void CalcularValorMedioConta()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 0)
            {
                // Calcula o valor médio da conta por iteração
                double valorTotalConta = 0.0;

                foreach (var conta in contas)
                {
                    // Certifica-se de que a conta seja do tipo correto antes de chamar o método
                    if (tipoDaConta == "energia" && conta is ContaEnergia contaEnergia)
                    {
                        valorTotalConta += contaEnergia.CalcularValorConta();
                    }
                    else if (tipoDaConta == "agua" && conta is ContaAgua contaAgua)
                    {
                        valorTotalConta += contaAgua.CalcularValorConta();
                    }
                }

                double valorMedioConta = valorTotalConta / contas.Count;

                Console.WriteLine($"Valor médio da conta de {tipoDaConta}: R$ {valorMedioConta:F2}");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui contas do tipo {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }

    static void EncontrarMaiorValorConta()
    {
        Console.WriteLine("Informe o identificador do cliente: ");
        string identificadorCliente = Console.ReadLine();

        if (clientes.ContainsKey(identificadorCliente))
        {
            Cliente cliente = clientes[identificadorCliente];

            Console.WriteLine("Escolha o tipo de conta (energia ou agua): ");
            string tipoDaConta = Console.ReadLine();

            // Verifica se o cliente possui contas do tipo especificado
            var contas = cliente.Contas.Where(c =>
                (tipoDaConta == "energia" && c is ContaEnergia) ||
                (tipoDaConta == "agua" && c is ContaAgua)).ToList();

            if (contas.Count > 0)
            {
                // Encontra a conta de maior valor por iteração
                double maiorValorConta = double.MinValue;

                foreach (var conta in contas)
                {
                    // Certifica-se de que a conta seja do tipo correto antes de chamar o método
                    if (tipoDaConta == "energia" && conta is ContaEnergia contaEnergia)
                    {
                        double valorConta = contaEnergia.CalcularValorConta();
                        if (valorConta > maiorValorConta)
                        {
                            maiorValorConta = valorConta;
                        }
                    }
                    else if (tipoDaConta == "agua" && conta is ContaAgua contaAgua)
                    {
                        double valorConta = contaAgua.CalcularValorConta();
                        if (valorConta > maiorValorConta)
                        {
                            maiorValorConta = valorConta;
                        }
                    }
                }

                Console.WriteLine($"Maior valor da conta de {tipoDaConta}: R$ {maiorValorConta:F2}");
            }
            else
            {
                Console.WriteLine($"Este cliente não possui contas do tipo {tipoDaConta}.");
            }
        }
        else
        {
            Console.WriteLine("Identificador de cliente não encontrado.");
        }
    }
}