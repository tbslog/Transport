import { useState, useEffect, useMemo } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import Select from "react-select";

const customStyles = {
  control: (provided, state) => ({
    ...provided,
    background: "#fff",
    borderColor: "#9e9e9e",
    minHeight: "30px",
    height: "30px",
    boxShadow: state.isFocused ? null : null,
  }),

  valueContainer: (provided, state) => ({
    ...provided,
    height: "30px",
    padding: "0 6px",
  }),

  input: (provided, state) => ({
    ...provided,
    margin: "0px",
  }),
  indicatorSeparator: (state) => ({
    display: "none",
  }),
  indicatorsContainer: (provided, state) => ({
    ...provided,
    height: "30px",
  }),
};

const PriceListContract = (props) => {
  const { selectIdClick, onlyCT, title } = props;

  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const columns = useMemo(() => [
    {
      name: "ID",
      selector: (row) => row.id,
      sortable: true,
    },
    {
      name: <div>Mã Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.maKh}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
      sortable: true,
    },
    {
      name: <div>Account</div>,
      selector: (row) => <div className="text-wrap">{row.accountName}</div>,
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.maHopDong}</div>,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => <div className="text-wrap">{row.soHopDongCha}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Đóng Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemDau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.diemCuoi}</div>,
      sortable: true,
    },

    {
      name: <div>Điểm Lấy/Trả Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.diemLayTraRong}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.donGia.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
      sortable: true,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => (
        <div className="text-wrap">{row.maLoaiPhuongTien}</div>
      ),
      sortable: true,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => <div className="text-wrap">{row.maLoaiHangHoa}</div>,
      sortable: true,
    },
    {
      name: <div>Đơn Vị Tính</div>,
      selector: (row) => <div className="text-wrap">{row.maDVT}</div>,
      sortable: true,
    },
    // {
    //   name: "Phương Thức Vận Chuyển",
    //   selector: (row) => row.maPTVC,
    //   sortable: true,
    // },
    {
      name: <div>Ngày Áp Dụng</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.ngayApDung).format("DD/MM/YYYY")}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Ngày Hết Hiệu Lực</div>,
      selector: (row) =>
        !row.ngayHetHieuLuc ? (
          ""
        ) : (
          <div className="text-wrap">{row.ngayHetHieuLuc}</div>
        ),
      sortable: true,
    },
  ]);

  const [page, setPage] = useState(1);
  const [IsLoading, SetIsLoading] = useState(false);
  const [selectedId, setSelectedId] = useState({});
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");
  const [loading, setLoading] = useState(false);

  const [listVehicleType, setListVehicleType] = useState([]);
  const [vehicleType, setVehicleType] = useState("");
  const [listGoodsType, setListGoodsType] = useState([]);
  const [goodsType, setGoodsType] = useState("");

  const [listFPlace, setListFPlace] = useState([]);
  const [listSPlace, setListSPlace] = useState([]);
  const [listEPlace, setListEPlace] = useState([]);
  const [listFPlaceSelected, setListFPlaceSelected] = useState([]);
  const [listSPlaceSelected, setListSPlaceSelected] = useState([]);
  const [listEPlaceSelected, setListEPlaceSelected] = useState([]);

  const [listAccountCus, setListAccountCus] = useState([]);
  const [listAccountSelected, setListAccountSelected] = useState([]);

  useEffect(() => {
    (async () => {
      const getListPlace = await getData(
        "Address/GetListAddressSelect?pointType=&type="
      );

      let arrPlace = [];
      getListPlace.forEach((val) => {
        arrPlace.push({
          label: val.tenDiaDiem + " - " + val.loaiDiaDiem,
          value: val.maDiaDiem,
        });
      });

      setListFPlace(arrPlace);
      setListSPlace(arrPlace);
      setListEPlace(arrPlace);

      let getListCustommerGroup = await getData(`Common/GetListVehicleType`);
      setListVehicleType(getListCustommerGroup);

      let getListCustommerType = await getData(`Common/GetListGoodsType`);
      setListGoodsType(getListCustommerType);
    })();
  }, []);

  useEffect(() => {
    if (
      props &&
      selectIdClick &&
      title &&
      Object.keys(selectIdClick).length > 0
    ) {
      setSelectedId(selectIdClick);
      (async () => {
        console.log(selectIdClick);
        const getListAcc = await getData(
          `AccountCustomer/GetListAccountSelectByCus?cusId=${selectIdClick.maKH}`
        );
        if (getListAcc && getListAcc.length > 0) {
          var obj = [];
          getListAcc.map((val) => {
            obj.push({
              value: val.accountId,
              label: val.accountId + " - " + val.accountName,
            });
          });
          setListAccountCus(obj);
        } else {
          setListAccountCus([]);
        }
      })();
    }
  }, [props, selectIdClick, title]);

  useEffect(() => {
    if (selectedId && Object.keys(selectedId).length > 0) {
      fetchData(1);
    }
  }, [selectedId]);

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);
    if (selectedId && Object.keys(selectedId).length > 0) {
      let listFilter = {
        listDiemDau: listFPlaceSelected,
        listDiemCuoi: listSPlaceSelected,
        listDiemLayTraRong: listEPlaceSelected,
        accountIds: listAccountSelected,
      };

      const dataCus = await getDataCustom(
        `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&goodsType=${goodsType}&vehicleType=${vehicleType}&onlyct=${onlyCT}`,
        listFilter
      );
      setData(dataCus.data);
      setPerPage(newPerPage);
      setTotalRows(dataCus.totalRecords);
      SetIsLoading(false);
    }
  };
  const fetchData = async (
    page,
    KeyWord = "",
    vehicleType = "",
    goodsType = "",
    listFPlace = [],
    listSPlace = [],
    listEPlace = [],
    listAccountSelected = []
  ) => {
    SetIsLoading(true);
    if (selectedId && Object.keys(selectedId).length > 0) {
      let listFilter = {
        listDiemDau: listFPlace,
        listDiemCuoi: listSPlace,
        listDiemLayTraRong: listEPlace,
        accountIds: listAccountSelected,
      };

      const dataCus = await getDataCustom(
        `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&goodsType=${goodsType}&vehicleType=${vehicleType}&onlyct=${onlyCT}`,
        listFilter
      );
      setData(dataCus.data);
      setTotalRows(dataCus.totalRecords);
      SetIsLoading(false);
    }
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(
      page,
      keySearch,
      vehicleType,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
  };
  const handleOnChangeVehicleType = (value) => {
    setLoading(true);
    setVehicleType(value);
    fetchData(
      page,
      keySearch,
      value,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
    setLoading(false);
  };

  const handleOnChangeGoodsType = (value) => {
    setLoading(true);
    setGoodsType(value);
    fetchData(
      page,
      keySearch,
      vehicleType,
      value,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
    setLoading(false);
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
      setLoading(true);

      let arrFPlace = [];
      let arrSPlace = [];
      let arrEplace = [];
      let arrAcc = [];

      if (type === "accountCus") {
        setValue("listAccountCus", values);

        values.forEach((val) => {
          arrAcc.push(val.value);
        });

        setListAccountSelected(arrAcc);
      } else {
        listAccountSelected.forEach((val) => {
          arrAcc.push(val);
        });
      }

      if (type === "fPlace") {
        setValue("ListFirstPlace", values);
        values.forEach((val) => {
          arrFPlace.push(val.value);
        });

        setListFPlaceSelected(arrFPlace);
      } else {
        listFPlaceSelected.forEach((val) => {
          arrFPlace.push(val);
        });
      }

      if (type === "sPlace") {
        setValue("ListSecondPlace", values);

        values.forEach((val) => {
          arrSPlace.push(val.value);
        });
        setListSPlaceSelected(arrSPlace);
      } else {
        listSPlaceSelected.forEach((val) => {
          arrSPlace.push(val);
        });
      }

      if (type === "ePlace") {
        setValue("ListEmptyPlace", values);

        values.forEach((val) => {
          arrEplace.push(val.value);
        });
        setListEPlaceSelected(arrEplace);
      } else {
        listEPlaceSelected.forEach((val) => {
          arrEplace.push(val);
        });
      }

      fetchData(
        page,
        keySearch,
        vehicleType,
        goodsType,
        arrFPlace,
        arrSPlace,
        arrEplace,
        arrAcc
      );
      setLoading(false);
    }
  };

  const handleRefeshDataClick = () => {
    fetchData(1);
    setPerPage(10);
    setKeySearch("");
    setGoodsType("");
    setVehicleType("");
  };

  const handleSearchClick = () => {
    fetchData(
      page,
      keySearch,
      vehicleType,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
  };

  return (
    <div>
      <div className="card card-default">
        <div className="card-header">
          {/* <h3 className="card-title">{title}</h3> */}
          <div className="container-fruid">
            <div className="row">
              <div className="col col-sm">
                <div className="row">
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <select
                        className="form-control form-control-sm"
                        onChange={(e) =>
                          handleOnChangeVehicleType(e.target.value)
                        }
                        value={vehicleType}
                      >
                        <option value="">Phương tiện vận tải</option>
                        {listVehicleType &&
                          listVehicleType.map((val) => {
                            return (
                              <option
                                value={val.maLoaiPhuongTien}
                                key={val.maLoaiPhuongTien}
                              >
                                {val.tenLoaiPhuongTien}
                              </option>
                            );
                          })}
                      </select>
                    </div>
                  </div>
                  <div className="col col-sm">
                    <div className="input-group input-group-sm">
                      <select
                        className="form-control form-control-sm"
                        onChange={(e) =>
                          handleOnChangeGoodsType(e.target.value)
                        }
                        value={goodsType}
                      >
                        <option value="">Loại Hàng Hóa</option>
                        {listGoodsType &&
                          listGoodsType.map((val) => {
                            return (
                              <option
                                value={val.maLoaiHangHoa}
                                key={val.maLoaiHangHoa}
                              >
                                {val.tenLoaiHangHoa}
                              </option>
                            );
                          })}
                      </select>
                    </div>
                  </div>
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <Controller
                    name="listAccountCus"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        className="basic-multi-select"
                        classNamePrefix={"form-control"}
                        isMulti
                        value={field.value}
                        options={listAccountCus}
                        styles={customStyles}
                        onChange={(field) =>
                          handleOnChangeFilterSelect(field, "accountCus")
                        }
                        placeholder="Chọn Account"
                      />
                    )}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <Controller
                    name="ListFirstPlace"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        className="basic-multi-select"
                        classNamePrefix={"form-control"}
                        isMulti
                        value={field.value}
                        options={listFPlace}
                        styles={customStyles}
                        onChange={(field) =>
                          handleOnChangeFilterSelect(field, "fPlace")
                        }
                        placeholder="Chọn Điểm Đầu"
                      />
                    )}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <Controller
                    name="ListSecondPlace"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        className="basic-multi-select"
                        classNamePrefix={"form-control"}
                        isMulti
                        value={field.value}
                        options={listSPlace}
                        styles={customStyles}
                        onChange={(field) =>
                          handleOnChangeFilterSelect(field, "sPlace")
                        }
                        placeholder="Chọn Điểm Cuối"
                      />
                    )}
                  />
                </div>
              </div>
              <div className="col col-sm">
                <div className="form-group">
                  <Controller
                    name="ListEmptyPlace"
                    control={control}
                    render={({ field }) => (
                      <Select
                        {...field}
                        className="basic-multi-select"
                        classNamePrefix={"form-control"}
                        isMulti
                        value={field.value}
                        options={listEPlace}
                        styles={customStyles}
                        onChange={(field) =>
                          handleOnChangeFilterSelect(field, "ePlace")
                        }
                        placeholder="Chọn Điểm Lấy/Trả rỗng"
                      />
                    )}
                  />
                </div>
              </div>
              <div className="col col-sm ">
                <div className="input-group input-group-sm">
                  <input
                    type="text"
                    className="form-control"
                    value={keySearch}
                    onChange={(e) => setKeySearch(e.target.value)}
                  />
                  <span className="input-group-append">
                    <button
                      type="button"
                      className="btn btn-sm btn-default"
                      onClick={() => handleSearchClick()}
                    >
                      <i className="fas fa-search"></i>
                    </button>
                  </span>
                  <button
                    type="button"
                    className="btn btn-sm btn-default mx-2"
                    onClick={() => handleRefeshDataClick()}
                  >
                    <i className="fas fa-sync-alt"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={IsLoading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PriceListContract;
