﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GISAServer.Hibernate.Objects {

    class LicencaObraDataLicencaConstrucaoEntity {

        private System.Int64 _Id;
        private FRDBaseEntity _FRDBase;
        private string _Ano;
        private string _Mes;
        private string _Dia;

        private System.Boolean _IsDeleted;
        private System.Byte[] _Versao;

        public virtual System.Int64 Id {
            get {
                return _Id;
            }
            set {
                _Id = value;
            }
        }

        public virtual FRDBaseEntity FRDBase {
            get {
                return _FRDBase;
            }
            set {
                _FRDBase = value;
            }
        }

        public virtual System.Boolean IsDeleted {
            get {
                return _IsDeleted;
            }
            set {
                _IsDeleted = value;
            }
        }

        public virtual System.Byte[] Versao {
            get {
                return _Versao;
            }
            set {
                _Versao = value;
            }
        }

        public virtual string Ano {
            get {
                return _Ano;
            }
            set {
                _Ano = value;
            }
        }

        public virtual string Mes {
            get {
                return _Mes;
            }
            set {
                _Mes = value;
            }
        }

        public virtual string Dia {
            get {
                return _Dia;
            }
            set {
                _Dia = value;
            }
        }

        protected bool Equals(LicencaObraDataLicencaConstrucaoEntity entity) {
            if (entity == null) return false;
            if (!base.Equals(entity)) return false;
            if (!Equals(_Id, entity._Id)) return false;
            return true;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as LicencaObraDataLicencaConstrucaoEntity);
        }

        public override int GetHashCode() {
            int result = base.GetHashCode();
            result = 29 * result + _Id.GetHashCode();
            return result;
        }

    }
}
