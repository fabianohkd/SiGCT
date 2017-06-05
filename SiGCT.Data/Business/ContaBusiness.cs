﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Microsoft.VisualBasic.FileIO;
using NHibernate.Helper.Generics;
using SiGCT.Data.DAO;
using SiGCT.Models;

namespace SiGCT.Data.Business
{
    public class ContaBusiness : GenericBusiness<long, Conta, ContaDAO>
    {
        #region Fields
        /// <summary>
        /// Framework para log <seealso cref="log4net"/>
        /// </summary>
        private static log4net.ILog logger = LogManager.GetLogger(System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString());

        private static Conta _conta;
        #endregion

        #region Constructor
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ContaBusiness() { }
        #endregion

        #region Public Methods
        /// <summary>
        /// Metodo para ler o arquivo na versão V3R0
        /// </summary>
        /// <param name="path">nome do arquivo</param>
        /// <returns></returns>
        public bool LerArquivoV3R0()
        {
            var path = @"C:\Users\fabiano.conrado\Desktop\Contas\Claro\2017\2017\04\612341225_140553264_53_04_2017_FebrabanV3.txt";
            if (File.Exists(path))
            {
                using (var file = new TextFieldParser(path, Encoding.UTF8))
                {
                    file.TextFieldType = FieldType.FixedWidth;
                    while (!file.EndOfData)
                    {
                        try
                        {
                            var rowType = file.PeekChars(2);
                            switch (rowType)
                            {
                                case "00": lerHeader(file); break;
                                case "10": lerResumo(file); break;
                                case "20": break;
                                case "30": break;
                                case "40": break;
                                case "50": break;
                                case "60": break;
                                case "70": lerAjuste(file); break;
                                case "80": break;
                                case "90": break;
                                case "99": break;
                                default: //TODO : Verifica o que fazer aqui
                                    break;
                            }
                        }
                        catch (MalformedLineException ex)
                        {
                            logger.Error(String.Format("Erro na linha '{0}' no arquivo '{1}', a linha não está em formato valido", file.LineNumber, path), ex);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(String.Format("Erro na linha '{0}' no arquivo '{1}' ", file.LineNumber, path), ex);
                        }
                    }
                    //return _listaBaixa;
                }
            }
            return false;
        }

        /// <summary>
        /// Detalhamento financeiros de movimentos anteriores - tipo 70
        /// </summary>
        /// <param name="file"></param>
        private void lerAjuste(TextFieldParser file)
        {
            var param = new int[] { 2, 12, 25, 8, 6, 25, 16, 1, 3, 3, 40, 13, 5, 1, 13, 8, 6, 8, 6, 123, 25, 1 };
            file.SetFieldWidths(param);
            var ajuste = file.ReadFields();

            _conta.Ajustes.Add(new AjusteBusiness().Parse(ajuste));
        }

        /// <summary>
        /// Somatório dos valores por recurso - tipo 10
        /// </summary>
        /// <param name="file"></param>
        private void lerResumo(TextFieldParser file)
        {
            var param = new int[] { 2, 12, 25, 8, 6, 25, 5, 16, 4, 8, 8, 9, 13, 9, 15, 13, 13, 2, 5, 4, 8, 114, 25, 1 };
            file.SetFieldWidths(param);
            var header = file.ReadFields();

            _conta.Resumos.Add(new ResumoBusiness().Parse(header));
        }

        /// <summary>
        /// Identificação geral da fatura de cobrança - tipo 00
        /// </summary>
        /// <param name="file"></param>
        /// <remarks>
        /// Apenas um por conta
        /// </remarks>
        private void lerHeader(TextFieldParser file)
        {
            var param = new int[] { 2, 12, 25, 8, 6, 8, 8, 3, 15, 15, 2, 15, 30, 15, 4, 16, 50, 2, 20, 4, 4, 10, 35, 15, 25, 1 };
            file.SetFieldWidths(param);
            var header = file.ReadFields();

            var _conta = Parse(header);
        }

        public Conta Parse(string[] array)
        {
            var conta = new Conta();
            conta.Identificador = array[2];
            conta.DataEmissao = DateTime.ParseExact(array[3], "yyyyMMdd", null);
            conta.DataArquivo = DateTime.ParseExact(array[5], "yyyyMMdd", null);
            conta.Vencimento = DateTime.ParseExact(array[6], "yyyyMMdd", null);
            conta.Operadora = new OperadoraBusiness().SaveAndReturn(array[7], array[8], array[9], array[10]);
            conta.Cliente = new ClienteBusiness().SaveAndReturn(array[11], array[12], array[13]);
            conta.Fatura = new FaturaBusiness().SaveAndReturn(array[15], array[16]);
            conta.Cobranca = new Cobranca() {
                //Tipo = new TipoCobrancaBusiness().SaveAndReturn(array[17], array[18]),
                //Banco = array[19],
                Agencia =  array[20],
                ContaCorrente = array[21]
            };
            conta.Fisco = array[22];
            conta.Filler = array[23];
            conta.Obs = array[24];

            return conta;
        }
        #endregion

    }
}
