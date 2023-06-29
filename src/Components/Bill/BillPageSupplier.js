import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, getFile } from "../Common/FuncAxios";
import { useForm, Controller } from "react-hook-form";
import Select from "react-select";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import DatePicker from "react-datepicker";
import { ToastError } from "../Common/FuncToast";
import LoadingPage from "../Common/Loading/LoadingPage";
import ListBillDetail from "./ListBillDetail";
import DetailBillById from "./DetailBillById";

const BillPageSupplier = (props) => {
  const { listBank } = props;
  const {
    control,
    setValue,
    watch,
    formState: { errors },
  } = useForm({
    mode: "onChange",
  });

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(30);
  const [page, setPage] = useState(1);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");
  const [datePay, setDatePay] = useState(new Date());

  const [listSupplier, setListSupplier] = useState([]);
  const [supSelected, setSupSelected] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => {
            handleButtonClick(val);
            SetShowModal("DetailBillByHandling");
          }}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hóa Đơn"
        >
          <i className="fas fa-file-invoice-dollar"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      selector: (row) => row.maDieuPhoi,
      omit: true,
    },
    {
      name: <div>Đơn Vị Vận Tải</div>,
      selector: (row) => <div className="text-wrap">{row.tenNCC}</div>,
    },
    {
      name: <div>Booking No</div>,
      selector: (row) => <div className="text-wrap">{row.maVanDonKH}</div>,
    },
    {
      name: <div>Hãng Tàu</div>,
      selector: (row) => <div className="text-wrap">{row.hangTau}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.loaiVanDon}</div>,
    },
    // {
    //   name: <div>Khách Hàng</div>,
    //   selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    // },
    // {
    //   name: <div>Account</div>,
    //   selector: (row) => <div className="text-wrap">{row.accountName}</div>,
    // },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },

    {
      name: <div>Loại Hàng Hóa</div>,
      selector: (row) => <div className="text-wrap">{row.loaiHangHoa}</div>,
    },
    {
      name: <div>Loại Phương Tiện</div>,
      selector: (row) => <div className="text-wrap">{row.loaiPhuongTien}</div>,
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },
    // {
    //   name: <div>Đơn Giá KH</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.donGiaKH.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    {
      name: <div>Đơn Giá</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.donGiaNCC.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Phụ Phí HĐ</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.chiPhiHopDong.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Phụ Phí Phát Sinh</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.chiPhiPhatSinh.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    // {
    //   name: <div>Mã Khách Hàng</div>,
    //   selector: (row) => <div className="text-wrap">{row.maKh}</div>,
    //   omit: true,
    // },

    // {
    //   name: <div>Account</div>,
    //   selector: (row) => <div className="text-wrap">{row.accountName}</div>,
    // },
    // {
    //   name: <div>Doanh Thu</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.doanhThu.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    // {
    //   name: <div>Lợi Nhuận</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.loiNhuan.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
  ]);

  useEffect(() => {
    (async () => {
      let getListCustomer = await getData(`Customer/GetListCustomerFilter`);
      if (getListCustomer && getListCustomer.length > 0) {
        let arrKh = [];
        getListCustomer
          .filter((x) => x.loaiKH === "NCC")
          .map((val) => {
            arrKh.push({
              label: val.tenKh,
              value: val.maKh,
            });
          });
        setListSupplier(arrKh);
      }
      await fetchData(1, "", "", "", "", datePay);
    })();
  }, []);

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

  const fetchData = async (
    page,
    KeyWord = "",
    fromDate,
    toDate,
    supSelected = "",
    datePay
  ) => {
    setLoading(true);
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataBills = await getData(
      `Bills/GetListBillHandling?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&date=${moment(
        new Date(datePay)
      ).format(
        "YYYY-MM-DD"
      )}&fromDate=${fromDate}&toDate=${toDate}&supplierId=${supSelected}&customerType=NCC`
    );

    setLoading(false);
    setData(dataBills.data);
    setTotalRows(dataBills.totalRecords);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(page, keySearch, fromDate, toDate, supSelected, datePay);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    const dataBills = await getData(
      `Bills/GetListBillHandling?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&date=${moment(
        new Date(datePay)
      ).format(
        "YYYY-MM-DD"
      )}&fromDate=${fromDate}&toDate=${toDate}&supplierId=${supSelected}&customerType=NCC`
    );
    setData(dataBills.data);
    setPerPage(newPerPage);
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleOnChangeFilterSelect = async (val) => {
    if (val) {
      setSupSelected(val.value);
      setValue("listCustomers", val);
      await fetchData(page, keySearch, fromDate, toDate, val.value, datePay);
    }
  };

  const handleButtonClick = async (value) => {
    setSelectIdClick(value);
    showModalForm();
  };

  const LoadBillOfCus = async (val) => {
    setDatePay(new Date(val));
    await fetchData(
      page,
      keySearch,
      fromDate,
      toDate,
      supSelected,
      new Date(val)
    );
  };

  const handleSearchClick = async () => {
    await fetchData(page, keySearch, fromDate, toDate, supSelected, datePay);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setData([]);
    await fetchData(1, "", "", "", "", datePay);
  };

  const handleExportExcel = async () => {
    if (!datePay) {
      ToastError("Vui lòng chọn mốc thời gian");
      return;
    }
    setLoading(true);

    let dateGet = moment(datePay).format("YYYY-MM-DD");
    const getFileDownLoad = await getFile(
      `Bills/ExportExcelBill?supplierId=${supSelected}&date=${dateGet}&customerType=${
        !supSelected ? "NCC" : ""
      }`,
      "HoaDon" + moment(new Date()).format("DD/MM/YYYY HHmmss")
    );
    setLoading(false);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Danh sách Chuyến Đã Hoàn Thành</h1>
            </div>
          </div>
        </div>
      </section>

      {loading && loading === true ? (
        <>
          <LoadingPage></LoadingPage>
        </>
      ) : (
        <section className="content">
          <div className="card">
            <div className="card-header">
              <div className="container-fruid">
                <div className="row">
                  <div className="col col-3">
                    <div className="col col-6">
                      <div className="form-group">
                        <Controller
                          name="listCustomers"
                          control={control}
                          render={({ field }) => (
                            <Select
                              {...field}
                              className="basic-multi-select"
                              classNamePrefix={"form-control"}
                              value={watch("listCustomers")}
                              options={listSupplier}
                              onChange={(field) =>
                                handleOnChangeFilterSelect(field)
                              }
                              placeholder="Chọn Đơn Vị Vận Tải"
                            />
                          )}
                        />
                      </div>
                    </div>
                    <div className="col col-2">
                      <button
                        type="button"
                        className="btn btn-title btn-sm btn-default mx-1"
                        gloss="Xem Hóa Đơn Kỳ"
                        onClick={() =>
                          showModalForm(SetShowModal("DetailBill"))
                        }
                      >
                        <i className="fas fa-money-bill-alt"></i>
                      </button>
                    </div>
                  </div>
                  <div className="col-sm-3"></div>
                  <div className="col-sm-3">
                    <div className="col col-sm"></div>
                    <div className="col col-sm">
                      <div className="input-group input-group-sm">
                        <DatePicker
                          selected={datePay}
                          onChange={(date) => LoadBillOfCus(date)}
                          dateFormat="MM/yyyy"
                          className="form-control form-control-sm"
                          placeholderText="Chọn Tháng"
                          value={datePay}
                          showMonthYearPicker
                          showFullMonthYearPicker
                        />
                      </div>
                    </div>
                  </div>
                  {/* <div className="col-sm-3">
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
                          showWeekNumbers
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
                          showWeekNumbers
                        />
                      </div>
                    </div>
                  </div>
                </div> */}

                  <div className="col-sm-3 ">
                    <div className="input-group input-group-sm">
                      <input
                        placeholder="Tìm Kiếm"
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

            <div className="card-body">
              <div className="container-datatable" style={{ height: "50vm" }}>
                <DataTable
                  columns={columns}
                  data={data}
                  progressPending={loading}
                  pagination
                  paginationServer
                  paginationTotalRows={totalRows}
                  paginationRowsPerPageOptions={[30, 50, 80, 100]}
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
                <div className="col-sm-3">
                  <button
                    // href={FileExcelImport}
                    onClick={() => handleExportExcel()}
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Tải File Excel"
                    type="button"
                  >
                    <i className="fas fa-file-excel"></i>
                  </button>
                  {/* <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    // onChange={(e) => handleExcelImportClick(e)}
                  />
                </div> */}
                </div>
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
                    {ShowModal === "DetailBill" && (
                      <ListBillDetail
                        customer={watch("listCustomers")}
                        datePay={datePay}
                        listBank={listBank}
                        cusType={"NCC"}
                      />
                    )}

                    {ShowModal === "DetailBillByHandling" && (
                      <DetailBillById
                        dataClick={selectIdClick}
                        listBank={listBank}
                        cusType={"NCC"}
                      />
                    )}
                  </>
                </div>
              </div>
            </div>
          </div>
        </section>
      )}
    </>
  );
};

export default BillPageSupplier;
