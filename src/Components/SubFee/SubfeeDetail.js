import { useMemo, useState, useEffect, useRef } from "react";
import { getData, getDataCustom, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Tab, Tabs, TabList, TabPanel } from "react-tabs";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import Select from "react-select";
import { useForm, Controller } from "react-hook-form";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import { ToastError } from "../Common/FuncToast";

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

const SubfeeDetail = (props) => {
  const { selectIdClick, title } = props;

  const columns = useMemo(() => [
    {
      name: <div>Mã phụ phí</div>,
      selector: (row) => row.priceId,
      sortable: true,
    },
    {
      name: <div>Loại Phụ Phí</div>,
      selector: (row) => <div className="text-wrap">{row.sfName}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Đối Tác</div>,
      selector: (row) => <div className="text-wrap">{row.customerName}</div>,
      sortable: true,
    },
    {
      name: <div>Account</div>,
      selector: (row) => <div className="text-wrap">{row.accountId}</div>,
      sortable: true,
    },
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.contractId}</div>,
      sortable: true,
    },
    {
      name: <div>Tên Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.contractName}</div>,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => <div className="text-wrap">{row.vehicleType}</div>,
    },
    {
      name: <div>Điểm Đóng Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.firstPlace}</div>,
    },
    {
      name: <div>Điểm Hạ Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.secondPlace}</div>,
    },
    {
      name: <div>Điểm Lấy/Trả Rỗng</div>,
      selector: (row) => <div className="text-wrap">{row.getEmptyPlace}</div>,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => row.goodsType,
    },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) =>
        row.unitPrice.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
    },
    {
      name: <div>Loại Tiền Tệ</div>,
      selector: (row) => row.priceType,
    },
    {
      name: <div>Thời gian Áp Dụng</div>,
      selector: (row) => row.approvedDate,
      sortable: true,
    },
    {
      name: <div>Thời gian Hết Hiệu Lực</div>,
      selector: (row) => row.deactiveDate,
      sortable: true,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => row.status,
      sortable: true,
    },
    {
      name: <div>Người Duyệt</div>,
      selector: (row) => row.approver,
    },
    {
      name: <div>Thời gian Tạo</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.createdTime).format("DD-MM-YYYY HH:mm:ss")}
        </div>
      ),
      sortable: true,
    },
  ]);

  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const [page, setPage] = useState(1);
  const [IsLoading, SetIsLoading] = useState(false);
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [contractId, setContractId] = useState("");

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

  const [ShowConfirm, setShowConfirm] = useState(false);
  const [toggledClearRows, setToggleClearRows] = useState(false);
  const [selectedRows, setSelectedRows] = useState([]);
  const [functionSubmit, setFunctionSubmit] = useState("");

  const [tabIndex, setTabIndex] = useState(0);

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
      (async () => {
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
        setContractId(selectIdClick.maHopDong);
        setTabIndex(1);
        fetchData(selectIdClick.maKH, selectIdClick.maHopDong, 1);
      })();
    }
  }, [props, selectIdClick, title]);

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);
    if (selectIdClick && Object.keys(selectIdClick).length > 0) {
      let listFilter = {
        listDiemDau: listFPlaceSelected,
        listDiemCuoi: listSPlaceSelected,
        listDiemLayTraRong: listEPlaceSelected,
        accountIds: listAccountSelected,
      };

      const dataCus = await getDataCustom(
        `SubFeePrice/GetListSubFeePriceByCustomer?customerId=${selectIdClick.maKH}&contractId=${contractId}&PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&goodsType=${goodsType}&vehicleType=${vehicleType}`,
        listFilter
      );
      setData(dataCus.data);
      setPerPage(newPerPage);
      setTotalRows(dataCus.totalRecords);
      SetIsLoading(false);
    }
  };

  const fetchData = async (
    customerId,
    contractId,
    page,
    KeyWord = "",
    vehicleType = "",
    goodsType = "",
    listFPlace = [],
    listSPlace = [],
    listEPlace = [],
    listAccount = []
  ) => {
    SetIsLoading(true);
    let listFilter = {
      listDiemDau: listFPlace,
      listDiemCuoi: listSPlace,
      listDiemLayTraRong: listEPlace,
      accountIds: listAccount,
    };

    const dataCus = await getDataCustom(
      `SubFeePrice/GetListSubFeePriceByCustomer?customerId=${customerId}&contractId=${contractId}&PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&goodsType=${goodsType}&vehicleType=${vehicleType}`,
      listFilter
    );
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(
      selectIdClick.maKH,
      contractId,
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
    SetIsLoading(true);
    setVehicleType(value);
    fetchData(
      selectIdClick.maKH,
      contractId,
      page,
      keySearch,
      value,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
    SetIsLoading(false);
  };

  const handleOnChangeGoodsType = (value) => {
    SetIsLoading(true);
    setGoodsType(value);
    fetchData(
      selectIdClick.maKH,
      contractId,
      page,
      keySearch,
      vehicleType,
      value,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected,
      listAccountSelected
    );
    SetIsLoading(false);
  };

  const ShowConfirmDialog = () => {
    if (selectedRows.length < 1) {
      ToastError("Vui lòng chọn phụ phí trước đã");
      return;
    } else {
      setShowConfirm(true);
    }
  };

  const DisableSubFee = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.priceId);
      });

      const SetApprove = await postData(`SubFeePrice/DisableSubFeePrice`, arr);

      if (SetApprove === 1) {
        fetchData(
          selectIdClick.maKH,
          contractId,
          1,
          keySearch,
          vehicleType,
          goodsType
        );
      }
      setSelectedRows([]);
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const handleChange = (state) => {
    setSelectedRows(state.selectedRows);
  };

  const handleClearRows = () => {
    setToggleClearRows(!toggledClearRows);
    setSelectedRows([]);
  };

  const DeleteSubFee = async () => {
    if (
      selectedRows &&
      selectedRows.length > 0 &&
      Object.keys(selectedRows).length > 0
    ) {
      let arr = [];
      selectedRows.map((val) => {
        arr.push(val.priceId);
      });

      const SetApprove = await postData(`SubFeePrice/DeleteSubFeePrice`, arr);

      if (SetApprove === 1) {
        fetchData(
          selectIdClick.maKH,
          contractId,
          1,
          keySearch,
          vehicleType,
          goodsType
        );
      }
      handleClearRows();
      setShowConfirm(false);
    }
  };

  const funcAgree = () => {
    if (functionSubmit && functionSubmit.length > 0) {
      switch (functionSubmit) {
        case "Disable":
          return DisableSubFee();
        case "Delete":
          return DeleteSubFee();
      }
    }
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
      SetIsLoading(true);

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
        selectIdClick.maKH,
        contractId,
        page,
        keySearch,
        vehicleType,
        goodsType,
        arrFPlace,
        arrSPlace,
        arrEplace,
        arrAcc
      );
      SetIsLoading(false);
    }
  };

  const handleRefeshDataClick = () => {
    setPerPage(10);
    setKeySearch("");
    setGoodsType("");
    setVehicleType("");

    setValue("listAccountCus", []);
    setValue("ListFirstPlace", []);
    setValue("ListSecondPlace", []);
    setValue("ListEmptyPlace", []);
    setTabIndex(1);
    fetchData(selectIdClick.maKH, contractId, 1, "", "", "");
  };

  const handleSearchClick = () => {
    fetchData(
      selectIdClick.maKH,
      contractId,
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

  const HandleOnChangeTabs = (tabIndex) => {
    setTabIndex(tabIndex);
    handleClearRows();
    let contractId = "";
    if (tabIndex === 0) {
      setContractId("");
      fetchData(selectIdClick.maKH, contractId, 1, "", "", "");
    }

    if (tabIndex === 1) {
      contractId = selectIdClick.maHopDong;
      setContractId(selectIdClick.maHopDong);
      fetchData(selectIdClick.maKH, contractId, 1, "", "", "");
    }
  };

  return (
    <div>
      <div className="card card-default">
        <div className="card-header">
          {/* <h3 className="card-title">{title}</h3> */}
          <div className="container-fruid">
            <div className="row">
              <div className="col col-sm">
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Vô Hiệu Phụ Phí"
                  onClick={() => {
                    setFunctionSubmit("Disable");
                    ShowConfirmDialog();
                  }}
                >
                  <i className="fas fa-eye-slash"></i>
                </button>
                <button
                  type="button"
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Xóa Phụ Phí"
                  onClick={() => {
                    setFunctionSubmit("Delete");
                    ShowConfirmDialog();
                  }}
                >
                  <i className="fas fa-trash-alt"></i>
                </button>
              </div>
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
            <Tabs
              selectedIndex={tabIndex}
              onSelect={(index) => HandleOnChangeTabs(index)}
            >
              <TabList>
                <Tab>Phụ Phí Hiện Hành</Tab>
                <Tab>Phụ Phí Theo Hợp Đồng</Tab>
              </TabList>
              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    columns={columns}
                    data={data}
                    progressPending={IsLoading}
                    pagination
                    paginationServer
                    paginationTotalRows={totalRows}
                    paginationRowsPerPageOptions={[10, 30, 50, 100]}
                    onSelectedRowsChange={handleChange}
                    selectableRows
                    onChangeRowsPerPage={handlePerRowsChange}
                    onChangePage={handlePageChange}
                    highlightOnHover
                    striped
                    direction="auto"
                    responsive
                    fixedHeader
                    dense
                    fixedHeaderScrollHeight="60vh"
                  />
                </div>
              </TabPanel>
              <TabPanel>
                <div className="container-datatable" style={{ height: "50vm" }}>
                  <DataTable
                    columns={columns}
                    data={data}
                    progressPending={IsLoading}
                    pagination
                    paginationServer
                    paginationTotalRows={totalRows}
                    paginationRowsPerPageOptions={[10, 30, 50, 100]}
                    onSelectedRowsChange={handleChange}
                    selectableRows
                    onChangeRowsPerPage={handlePerRowsChange}
                    onChangePage={handlePageChange}
                    highlightOnHover
                    striped
                    direction="auto"
                    responsive
                    fixedHeader
                    dense
                    fixedHeaderScrollHeight="60vh"
                  />
                </div>
              </TabPanel>
            </Tabs>
          </div>
        </div>
      </div>
      {ShowConfirm === true && (
        <ConfirmDialog
          setShowConfirm={setShowConfirm}
          title={"Bạn có chắc chắn với quyết định này?"}
          content={
            "Khi thực hiện hành động này sẽ không thể hoàn tác lại được nữa."
          }
          funcAgree={funcAgree}
        />
      )}
    </div>
  );
};

export default SubfeeDetail;
