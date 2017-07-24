using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct TsumTypeId {

    private int _id;   

    public TsumTypeId(int id) {
        _id = id;
    }

    public static bool operator == (TsumTypeId a, TsumTypeId b) {
        return a._id == b._id;
    }
    public static bool operator != (TsumTypeId a, TsumTypeId b) {
        return !(a == b);
    }

    public override bool Equals(object obj) {
        return this._id == ((TsumTypeId)obj)._id;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

    public override string ToString() {
        return  _id.ToString();
    }
}