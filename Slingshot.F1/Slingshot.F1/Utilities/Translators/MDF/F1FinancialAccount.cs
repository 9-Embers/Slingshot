﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

using Slingshot.Core;
using Slingshot.Core.Model;

namespace Slingshot.F1.Utilities.Translators.MDB
{
    public static class F1FinancialAccount
    {
        public static FinancialAccount Translate( DataRow row )
        {
            var account = new FinancialAccount();

            if( string.IsNullOrWhiteSpace( row.Field<string>( "sub_fund_name" ) ) )
            {
                account.Name = row.Field<string>( "fund_name" );

                //Use Hash to create Account ID
                MD5 md5Hasher = MD5.Create();
                var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( row.Field<string>( "fund_name" ) ) );
                var accountId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
                if ( accountId > 0 )
                {
                    account.ParentAccountId = accountId;
                }
            }
            else
            {
                account.Name = row.Field<string>( "sub_fund_name" );
               
                //Use Hash to get parent Account ID
                MD5 md5Hasher = MD5.Create();
                var hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( row.Field<string>( "fund_name" ) ) );
                var parentAccountId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
                if ( parentAccountId > 0 )
                {
                    account.ParentAccountId = parentAccountId;
                }

                //Use Hash to create Account ID
                hashed = md5Hasher.ComputeHash( Encoding.UTF8.GetBytes( row.Field<string>( "fund_name" ) + row.Field<string>( "sub_fund_name" ) ) );
                var accountId = Math.Abs( BitConverter.ToInt32( hashed, 0 ) ); // used abs to ensure positive number
                if ( accountId > 0 )
                {
                    account.ParentAccountId = accountId;
                }
            }

            account.IsTaxDeductible = row.Field<int>( "taxDeductible" ) != 0;

            return account;
        }
    }
}
