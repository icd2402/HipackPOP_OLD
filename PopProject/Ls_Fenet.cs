using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopProject
{
    class Ls_Fenet
    {
        public string LSIS_NUMBER = "LSIS-XGT";

        //LS XGT(XGB) FeNet 데이터 연속 읽기 ex) D4000 ~ D4059 60word 
        //PLC고유번호 : 4C 53 49 53 2D 58 47 54 00 00 (고정)
        //PLC정보 : 0000 (고정)
        //CPU정보 : B0 (고정)
        //프레임 방향 : 33(요청), 11(응답)
        //프레임 순서 번호 : 0001 (고정)
        //길이 : 1300
        //위치 : 00 (고정)
        //체크섬 : 3F (고정)
        //명령어 : 5400(읽기, 4800(쓰기)
        //데이터 타입 : 0000(비트), 0100(바이트), 0200(워드), 0300(더블워드), 0400(롱워드), 1400(연속 바이트)
        //예약 영역 : 0000 (고정)
        //블록 개수 : 0100
        //변수 길이 : 0700
        //데이터 주소 : 25 44 42 34 30 30 30
        //데이터개수 : 7800


        // 해더 포맷 // 176(CNS) , 160(로봇)
        public string head_format(byte variable_length)
        {
            string header_format = string.Empty;
            header_format += LSIS_NUMBER; // 고정값
            header_format += (char)(0x00); // 예약영역 
            header_format += (char)(0x00); // 예약영역 
            header_format += (char)(0x00); // PLC 정보
            header_format += (char)(0x00); // PLC 정보

            header_format += ((char)160).ToString(); // CPU 정보 B0 (XGB)
            header_format += (char)(0x33); // 프레임 방향 (11=응답, 33=요청) 
            header_format += (char)(0x00); // 프레임 순서 번호 
            header_format += (char)(0x01); // 프레임 순서 번호 
            header_format += (char)(variable_length); // 데이터 길이
            header_format += (char)(0x00); // 데이터 길이
            header_format += (char)(0x01); ; // FeNet 슬롯 번호
            header_format += (char)(0x00); ; // 체크섬

            return header_format;
        }
        public byte[] Word_single_Write(string variableName, byte datatype, int writeData)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x58); // 쓰기 명령어 상위
            instruction_format += (char)(0x00); // 쓰기 명령어 하위  
            instruction_format += (char)(datatype); // 데이터 타입 상위 (00: bit, 01: byte, 02: WORD, 03: DWROD, 04: LWROD)
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위  

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위 
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위 
            variable += (char)(0x00); // 직접변수 길이 하위 
            variable += variableName; // 직접변수
            variable += (char)(0x01); // 데이터 개수 상위 
            variable += (char)(0x00); // 데이터 개수 하위 

            // 쓸 데이터
            string Writedata = writeData.ToString("X2"); // Dec를 hex로 변환
            string Wdata = Writedata.PadLeft(4, '0'); //4자리 hex로 변환
            string low_data = Wdata.Substring(0, 2); //4자리 hex중 앞 두자리
            string high_data = Wdata.Substring(2, 2); //4자리 hex중 뒤 두자리
            variable += (char)(Convert.ToInt32(high_data.ToString(), 16)); //데이터 상위
            variable += (char)(Convert.ToInt32(low_data.ToString(), 16)); //데이터 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length); ;

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }

        public byte[] Byte_single_Read(string variableName)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x54); // 읽기 명령어 상위
            instruction_format += (char)(0x00); // 읽기 명령어 하위
            instruction_format += (char)(0x00); // 데이터 타입 상위 (00: bit, 01: byte, 02: WORD, 03: DWROD, 04: LWROD)
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위 

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위 
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위
            variable += (char)(0x00); // 변수 길이 하위
            variable += variableName; // 변수이름
            variable += (char)(0x01); // 읽을 데이터 개수 상위
            variable += (char)(0x00); // 읽을 데이터 개수 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length); ;

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }


        public byte[] Byte_continuous_Read(string variable_Type, string variableName, int read_qty)
        {
            //명령어
            string instruction_format = string.Empty;

            instruction_format += (char)(0x54); // 읽기 명령어 상위
            instruction_format += (char)(0x00); // 읽기 명령어 하위
            instruction_format += (char)(0x14); // 데이터 타입 상위
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위 

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            string Name = Convert.ToString((Convert.ToInt32(variableName) * 2));//

            variable += (char)(0x01); // 블록수 상위
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(Name.Length + 3); // 변수 길이 상위 
            variable += (char)(0x00); // 변수 길이 하위
            variable += (char)(0x25); // %
            variable += variable_Type; // 변수 타입 (M,D,Z 등...)
            variable += (char)(0x42); // D
            variable += Convert.ToString((Convert.ToInt32(variableName) * 2));
            variable += (char)(read_qty * 2); // 읽을 데이터 개수 상위
            variable += (char)(0x00); // 읽을 데이터 개수 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length);

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }
        //개별 읽기
        public byte[] single_Read(string variableName, byte datatype)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x54); // 읽기 명령어 상위
            instruction_format += (char)(0x00); // 읽기 명령어 하위
            instruction_format += (char)(datatype); // 데이터 타입 상위 (00: bit, 01: byte, 02: WORD, 03: DWROD, 04: LWROD)
            instruction_format += (char)(0x00); // 데이터 타입  하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위 

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위 
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위
            variable += (char)(0x00); // 변수 길이 하위
            variable += variableName; // 변수이름
            variable += (char)(0x01); // 읽을 데이터 개수 상위
            variable += (char)(0x00); // 읽을 데이터 개수 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length); ;

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }
        public byte[] Word_continuous_Read(string variableName, int read_qty)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x54); // 읽기 명령어 상위
            instruction_format += (char)(0x00); // 읽기 명령어 하위
            instruction_format += (char)(0x14); // 데이터 타입 상위
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위 

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위 
            variable += (char)(0x00); // 변수 길이 하위
            variable += variableName; // 변수이름
            variable += (char)(read_qty * 2); // 읽을 데이터 개수 상위
            variable += (char)(0x00); // 읽을 데이터 개수 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length);

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }
        // 연속 읽기
        public byte[] continuous_Read(string variableName, int read_qty)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x54); // 읽기 명령어 상위
            instruction_format += (char)(0x00); // 읽기 명령어 하위
            instruction_format += (char)(0x14); // 데이터 타입 상위
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위 

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위 
            variable += (char)(0x00); // 변수 길이 하위
            variable += variableName; // 변수이름
            variable += (char)(read_qty * 2); // 읽을 데이터 개수 상위
            variable += (char)(0x00); // 읽을 데이터 개수 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length);

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }

        //개별쓰기
        public byte[] single_Write(string variableName, byte datatype, int writeData)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x58); // 쓰기 명령어 상위
            instruction_format += (char)(0x00); // 쓰기 명령어 하위  
            instruction_format += (char)(datatype); // 데이터 타입 상위 (00: bit, 01: byte, 02: WORD, 03: DWROD, 04: LWROD)
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위
            instruction_format += (char)(0x00); // 예약 영역 하위  

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)(0x01); // 블록수 상위 
            variable += (char)(0x00); // 블록수 하위 
            variable += (char)(variableName.Length); // 변수 길이 상위 
            variable += (char)(0x00); // 직접변수 길이 하위 
            variable += variableName; // 직접변수
            variable += (char)(0x01); // 데이터 개수 상위 
            variable += (char)(0x00); // 데이터 개수 하위 

            // 쓸 데이터
            string Writedata = writeData.ToString("X2"); // Dec를 hex로 변환
            string Wdata = Writedata.PadLeft(4, '0'); //4자리 hex로 변환
            string low_data = Wdata.Substring(0, 2); //4자리 hex중 앞 두자리
            string high_data = Wdata.Substring(2, 2); //4자리 hex중 뒤 두자리
            variable += (char)(Convert.ToInt32(high_data.ToString(), 16)); //데이터 상위
            variable += (char)(Convert.ToInt32(low_data.ToString(), 16)); //데이터 하위

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            byte variable_length = byte_convert(variable.Length); ;

            instruction_format = instruction_format.Insert(0, head_format(variable_length));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }

        //연속 쓰기
        public byte[] continuous_Write(string variableName, int write_qty, List<int> items)
        {
            //명령어
            string instruction_format = string.Empty;
            instruction_format += (char)(0x58); // 쓰기 명령어 상위 
            instruction_format += (char)(0x00); // 쓰기 명령어 하위  
            instruction_format += (char)(0x14); // 데이터 타입 상위 (00: bit, 01: byte, 02: WORD, 03: DWROD, 04: LWROD)
            instruction_format += (char)(0x00); // 데이터 타입 하위  
            instruction_format += (char)(0x00); // 예약 영역 상위 
            instruction_format += (char)(0x00); // 예약 영역 하위  

            //변수 길이만 얻기위해 따로 분리.
            string variable = string.Empty;
            variable += (char)0x01; // 블록수 상위 
            variable += (char)0x00; // 블록수 하위
            variable += (char)variableName.Length; // 변수 길이 상위 
            variable += (char)0x00; // 변수 길이 하위
            variable += variableName; // PLC 첫번째 변수이름

            string hexa_qty = write_qty.ToString("x2"); //hex로 변환
            variable += (char)(Convert.ToInt32(hexa_qty.ToString(), 16)); // 데이터 개수 상위
            variable += (char)(0x00); // 데이터 개수 하위 
            foreach (var item in items)
            {
                string hexaString = item.ToString("x2"); //hex로 변환
                string Wdata = hexaString.PadLeft(4, '0'); //4자리 hex로 변환
                string low_data = Wdata.Substring(0, 2); //4자리 hex중 앞 두자리
                string high_data = Wdata.Substring(2, 2); //4자리 hex중 뒤 두자리
                variable += (char)(Convert.ToInt32(high_data.ToString(), 16)); //데이터 상위
                variable += (char)(Convert.ToInt32(low_data.ToString(), 16)); //데이터 상위
            }

            instruction_format += variable;

            //헤더에 변수길이값을 구하기 위함.
            string variable_length = instruction_format.Length.ToString("x2");
            var Variable_length = Convert.ToInt32(variable_length.ToString(), 16);

            instruction_format = instruction_format.Insert(0, head_format(Convert.ToByte(Variable_length)));// 헤더값 얻어오기
            byte[] bDts = Encoding.GetEncoding("iso-8859-1").GetBytes(instruction_format); //byte로 변환

            return bDts;
        }

        //legnth 길이 변환
        private byte byte_convert(int source)
        {
            string byte_change = string.Format("0x{0}", source);
            return Convert.ToByte(byte_change, 16);

        }

    }
}
