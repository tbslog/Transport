import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getDataCustom } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import AddPriceTable from "./AddPriceTable";
import ApprovePriceTable from "./ApprovePriceTable";
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

const PriceTablePage = () => {
  const {
    control,
    setValue,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(0);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [listStatus, setListStatus] = useState([]);
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

  const [title, setTitle] = useState("");

  const paginationComponentOptions = {
    rowsPerPageText: "Dữ liệu mỗi trang",
    rangeSeparatorText: "của",
    selectAllRowsItem: true,
    selectAllRowsItemText: "Tất cả",
  };

  const columns = useMemo(() => [
    {
      name: <div>Mã Hợp Đồng</div>,
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: <div>Tên Hợp Đồng</div>,
      selector: (row) => <div className="text-wrap">{row.tenHopDong}</div>,
    },
    {
      name: <div>Tên Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) =>
        row.donGia.toLocaleString("vi-VI", {
          style: "currency",
          currency: "VND",
        }),
    },
    {
      name: <div>Điểm Đóng Hàng</div>,
      selector: (row) => <div className="text-warp">{row.diemDau}</div>,
      sortable: true,
    },
    {
      name: <div>Điểm Trả Hàng</div>,
      selector: (row) => <div className="text-warp">{row.diemCuoi}</div>,
      sortable: true,
    },

    {
      name: <div>Điểm Lấy/Trả Rỗng</div>,
      selector: (row) => <div className="text-warp">{row.diemLayTraRong}</div>,
      sortable: true,
    },
    {
      name: <div>Phương Tiện Vận Tải</div>,
      selector: (row) => row.maLoaiPhuongTien,
    },
    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => row.maLoaiHangHoa,
    },
    {
      name: <div>Phương Thức Vận Chuyển</div>,
      selector: (row) => row.maPtvc,
    },
    {
      name: <div>Trạng Thái</div>,
      selector: (row) => row.trangThai,
    },
    {
      name: <div>Thời gian áp dụng</div>,
      selector: (row) => (
        <div className="text-wrap">
          {moment(row.ngayApDung).format("DD/MM/YYYY")}
        </div>
      ),
      sortable: true,
    },
    {
      name: <div>Thời gian hết hiệu lực</div>,
      selector: (row) =>
        !row.ngayHetHieuLuc ? (
          ""
        ) : (
          <div className="text-wrap">
            {moment(row.ngayHetHieuLuc).format("DD/MM/YYYY")}
          </div>
        ),
      sortable: true,
    },
  ]);

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const hideModal = () => {
    modal.hide();
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleEditButtonClick = async (val) => {
    showModalForm();
    setSelectIdClick(val);
  };

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate = "",
    toDate = "",
    vehicleType = "",
    goodsType = "",
    listFPlace = [],
    listSPlace = [],
    listEPlace = []
  ) => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    let listFilter = {
      listDiemDau: listFPlace,
      listDiemCuoi: listSPlace,
      listDiemLayTraRong: listEPlace,
    };

    const dataCus = await getDataCustom(
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}&goodsType=${goodsType}&vehicleType=${vehicleType}`,
      listFilter
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);

    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      vehicleType,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected
    );
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    let listFilter = {
      listDiemDau: listFPlaceSelected,
      listDiemCuoi: listSPlaceSelected,
      listDiemLayTraRong: listEPlaceSelected,
    };

    const dataCus = await getDataCustom(
      `PriceTable/GetListPriceTable?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}&goodsType=${goodsType}&vehicleType=${vehicleType}`,
      listFilter
    );
    setPerPage(newPerPage);
    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);

    setLoading(false);
  };

  useEffect(() => {
    setLoading(true);

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

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);
    })();

    fetchData(1);
    setLoading(false);
  }, []);

  const handleOnChangeVehicleType = (value) => {
    setLoading(true);
    setVehicleType(value);
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      value,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected
    );
    setLoading(false);
  };

  const handleOnChangeGoodsType = (value) => {
    setLoading(true);
    setGoodsType(value);
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      vehicleType,
      value,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected
    );
    setLoading(false);
  };

  const handleSearchClick = () => {
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      vehicleType,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected
    );
  };

  const ReloadData = () => {
    fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      vehicleType,
      goodsType,
      listFPlaceSelected,
      listSPlaceSelected,
      listEPlaceSelected
    );
  };

  const handleRefeshDataClick = () => {
    fetchData(1);
    setPerPage(10);
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setGoodsType("");
    setVehicleType("");
  };

  const getDataApprove = async () => {
    const listApprove = await getData(
      `PriceTable/GetListPriceTableApprove?PageNumber=1&PageSize=10`
    );
    return listApprove;
  };

  const handleOnChangeFilterSelect = async (values, type) => {
    if (values) {
      setLoading(true);

      let arrFPlace = [];
      let arrSPlace = [];
      let arrEplace = [];

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
        fromDate,
        toDate,
        vehicleType,
        goodsType,
        arrFPlace,
        arrSPlace,
        arrEplace
      );
      setLoading(false);
    }
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý bảng giá</h1>
            </div>
            {/* <div className="col-sm-6">
                  <ol className="breadcrumb float-sm-right">
                    <li className="breadcrumb-item">
                      <a href="#">Home</a>
                    </li>
                    <li className="breadcrumb-item active">Blank Page</li>
                  </ol>
                </div> */}
          </div>
        </div>
      </section>

      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                <div className="col col-sm">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Thêm Mới Bảng Giá"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setSelectIdClick({}),
                        setTitle("Tạo Mới Bảng Giá")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Duyệt Bảng Giá"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("ApprovePriceTable"),
                        setSelectIdClick({}),
                        setTitle("Duyệt Bảng Giá")
                      )
                    }
                  >
                    <i className="fas fa-check-double"></i>
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
                <div className="col col-sm">
                  <div className="row">
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <DatePicker
                          selected={fromDate}
                          onChange={(date) => setFromDate(date)}
                          dateFormat="dd/MM/yyyy"
                          className="form-control form-control-sm"
                          placeholderText="Từ ngày"
                          value={fromDate}
                        />
                      </div>
                    </div>
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <DatePicker
                          selected={toDate}
                          onChange={(date) => setToDate(date)}
                          dateFormat="dd/MM/yyyy"
                          className="form-control form-control-sm"
                          placeholderText="Đến Ngày"
                          value={toDate}
                        />
                      </div>
                    </div>
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
              <div className="row">
                <div className="col col-6"></div>
              </div>
            </div>
          </div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                title="Danh sách bảng giá"
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
                paginationRowsPerPageOptions={[10, 30, 50, 100]}
                onSelectedRowsChange={handleChange}
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
          <div className="card-footer">
            <div className="row">
              {/* <div className="col-sm-3">
                <a
                  title="Tải Template Excel"
                  href={FileExcelImport}
                  download="Template Thêm mới Khách hàng.xlsx"
                  className="btn btn-sm btn-default mx-1"
                >
                  <i className="fas fa-download"></i>
                </a>
                <div className="upload-btn-wrapper">
                  <button
                    className="btn btn-sm btn-default mx-1"
                    title="Upload file Excel"
                  >
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
              </div> */}
            </div>
          </div>
        </div>
        <div
          className="modal fade"
          id="modal-xl"
          data-backdrop="static"
          ref={parseExceptionModal}
          aria-labelledby="parseExceptionModal"
          backdrop="static"
        >
          <div
            className="modal-dialog modal-dialog-scrollable"
            style={{ maxWidth: "95%" }}
          >
            <div className="modal-content">
              <div className="modal-header">
                <h5>{title}</h5>
                <button
                  type="button"
                  className="close"
                  data-dismiss="modal"
                  onClick={() => hideModal()}
                  aria-label="Close"
                >
                  <span aria-hidden="true">×</span>
                </button>
              </div>
              <div className="modal-body">
                <>
                  {ShowModal === "Create" && (
                    <AddPriceTable
                      selectIdClick={selectIdClick}
                      listStatus={listStatus}
                    />
                  )}
                  {ShowModal === "ApprovePriceTable" && (
                    <ApprovePriceTable
                      reLoadData={ReloadData}
                      getDataApprove={getDataApprove}
                      checkShowModal={modal}
                    />
                  )}
                </>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default PriceTablePage;
