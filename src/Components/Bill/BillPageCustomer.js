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

const customStyles = {
  rows: {
    style: {
      backgroundColor: "#EFE5D0",
    },
  },
  headCells: {
    style: {
      backgroundColor: "#EFE5D0",
    },
  },
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

const BillPageCustomer = (props) => {
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

  const [listCustomer, setListCustomer] = useState([]);
  const [cusSelected, setCusselected] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() =>
            handleButtonClick(val, SetShowModal("DetailBillByTransport"))
          }
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hóa Đơn "
        >
          <i className="fas fa-file-invoice-dollar"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      selector: (row) => row.maVanDon,
      omit: true,
    },
    {
      name: <div>Booking No</div>,
      selector: (row) => <div className="text-wrap">{row.bookingNo}</div>,
    },
    {
      name: <div>Hãng Tàu</div>,
      selector: (row) => <div className="text-wrap">{row.hangTau}</div>,
    },
    {
      name: <div>Loại Vận Đơn</div>,
      selector: (row) => <div className="text-wrap">{row.loaiVanDon}</div>,
    },
    {
      name: <div>Khách Hàng</div>,
      selector: (row) => <div className="text-wrap">{row.tenKH}</div>,
    },
    {
      name: <div>Account</div>,
      selector: (row) => <div className="text-wrap">{row.accountName}</div>,
    },
    {
      name: <div>PTVC</div>,
      selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    },

    {
      name: <div>Tổng Tiền</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.tongTien.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },
    {
      name: <div>Tổng Phụ Phí</div>,
      selector: (row) => (
        <div className="text-wrap">
          {row.tongPhuPhi.toLocaleString("vi-VI", {
            style: "currency",
            currency: "VND",
          })}
        </div>
      ),
    },

    // {
    //   name: <div>Loại Hàng Hóa</div>,
    //   selector: (row) => <div className="text-wrap">{row.loaiHangHoa}</div>,
    // },
    // {
    //   name: <div>Loại Phương Tiện</div>,
    //   selector: (row) => <div className="text-wrap">{row.loaiPhuongTien}</div>,
    // },
    // {
    //   name: <div>PTVC</div>,
    //   selector: (row) => <div className="text-wrap">{row.maPTVC}</div>,
    // },
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
    //   name: <div>Đơn Vị Vận Tải</div>,
    //   selector: (row) => <div className="text-wrap">{row.tenNCC}</div>,
    // },
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
    // {
    //   name: <div>Đơn Giá NCC</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.donGiaNCC.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
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
    // {
    //   name: <div>Phụ Phí HĐ</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.chiPhiHopDong.toLocaleString("vi-VI", {
    //         style: "currency",
    //         currency: "VND",
    //       })}
    //     </div>
    //   ),
    // },
    // {
    //   name: <div>Phụ Phí Phát Sinh</div>,
    //   selector: (row) => (
    //     <div className="text-wrap">
    //       {row.chiPhiPhatSinh.toLocaleString("vi-VI", {
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
          .filter((x) => x.loaiKH === "KH")
          .map((val) => {
            arrKh.push({
              label: val.tenKh,
              value: val.maKh,
            });
          });
        setListCustomer(arrKh);

        await fetchData(1, "", "", "", "", new Date());
      }
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
    customerId = "",
    datePay
  ) => {
    setLoading(true);
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }
    fromDate = !fromDate ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = !toDate ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataBills = await getData(
      `Bills/ListBillByTransport?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&date=${moment(
        new Date(datePay)
      ).format(
        "YYYY-MM-DD"
      )}&fromDate=${fromDate}&toDate=${toDate}&customerId=${customerId}&customerType=KH`
    );

    setLoading(false);
    setData(dataBills.data);
    setTotalRows(dataBills.totalRecords);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(page, keySearch, fromDate, toDate, cusSelected, datePay);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    const dataBills = await getData(
      `Bills/ListBillByTransport?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&date=${moment(
        new Date(datePay)
      ).format(
        "YYYY-MM-DD"
      )}&fromDate=${fromDate}&toDate=${toDate}&customerId=${cusSelected}&customerType=KH`
    );
    setData(dataBills.data);
    setPerPage(newPerPage);
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleOnChangeFilterSelect = async (val) => {
    if (val) {
      setValue("listCustomers", val);
      setCusselected(val.value);
      await fetchData(page, keySearch, fromDate, toDate, val.value, datePay);
    }
  };

  const handleButtonClick = (value) => {
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
      cusSelected,
      new Date(val)
    );
  };

  const handleSearchClick = () => {
    setLoading(true);
    fetchData(page, keySearch, fromDate, toDate, cusSelected, datePay);
    setLoading(false);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    setFromDate("");
    setToDate("");
    setData([]);
    await fetchData(1, "", "", "", cusSelected, new Date());
  };

  const ExpandedComponent = ({ data }) => {
    if (data.listBillHandlingWebs && data.listBillHandlingWebs.length > 0) {
      return (
        <div className="container-datatable" style={{ height: "50vm" }}>
          <DataTable
            columns={[
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
                selector: (row) => row.handlingId,
                omit: true,
              },
              {
                name: "Đơn Vị Vận Tải",
                selector: (row) => row.donViVanTai,
              },
              {
                name: "Điểm Đóng Hàng",
                selector: (row) => row.diemDau,
              },
              {
                name: "Điểm Hạ Hàng",
                selector: (row) => row.diemCuoi,
              },
              {
                name: "Điểm Lấy Rỗng",
                selector: (row) => row.diemLayRong,
              },
              {
                name: "Điểm Trả Rỗng",
                selector: (row) => row.diemTraRong,
              },
              {
                name: "Loại Hàng Hóa",
                selector: (row) => row.loaiHangHoa,
              },
              {
                name: "Loại Phương Tiện",
                selector: (row) => row.loaiPhuongTien,
              },
              {
                name: "Mã Cont",
                selector: (row) => row.contNo,
              },
              {
                name: "Mã Số Xe",
                selector: (row) => row.maSoXe,
              },
              {
                name: "Tài Xế",
                selector: (row) => row.taiXe,
              },
              // {
              //   name: "Đơn Giá",
              //   selector: (row) => row.donGiaKH,
              // },
              // {
              //   name: "PP Hợp Đồng",
              //   selector: (row) => row.phuPhiHD,
              // },
              // {
              //   name: "PP Phát Sinh",
              //   selector: (row) => row.phuPhiPhatSinh,
              // },
              {
                name: <div>Đơn Giá</div>,
                selector: (row) => (
                  <div className="text-wrap">
                    {row.donGiaKH.toLocaleString("vi-VI", {
                      style: "currency",
                      currency: "VND",
                    })}
                  </div>
                ),
              },
              {
                name: <div>PP Hợp Đồng</div>,
                selector: (row) => (
                  <div className="text-wrap">
                    {row.phuPhiHD.toLocaleString("vi-VI", {
                      style: "currency",
                      currency: "VND",
                    })}
                  </div>
                ),
              },
              {
                name: <div>PP Phát Sinh</div>,
                selector: (row) => (
                  <div className="text-wrap">
                    {row.phuPhiPhatSinh.toLocaleString("vi-VI", {
                      style: "currency",
                      currency: "VND",
                    })}
                  </div>
                ),
              },
            ]}
            data={data.listBillHandlingWebs}
            highlightOnHover
            direction="auto"
            customStyles={customStyles}
            responsive
          />
        </div>
      );
    }
  };

  const handleExportExcel = async () => {
    if (!datePay) {
      ToastError("Vui lòng chọn mốc thời gian");
      return;
    }
    setLoading(true);

    let dateGet = moment(datePay).format("YYYY-MM-DD");
    const getFileDownLoad = await getFile(
      `Bills/ExportExcelBill?customerId=${cusSelected}&date=${dateGet}&customerType=${
        !cusSelected ? "KH" : ""
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

      {loading === true ? (
        <>
          <LoadingPage></LoadingPage>
        </>
      ) : (
        <>
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
                                options={listCustomer}
                                onChange={(field) =>
                                  handleOnChangeFilterSelect(field)
                                }
                                placeholder="Chọn Khách Hàng"
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
                    <div className="col-sm-2">
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

                    <div className="col-sm-4">
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
                    expandableRows
                    expandableRowsComponent={ExpandedComponent}
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
                          cusType={"KH"}
                        />
                      )}
                      {ShowModal === "DetailBillByTransport" && (
                        <DetailBillById
                          dataClick={selectIdClick}
                          listBank={listBank}
                          cusType={"KH"}
                        />
                      )}
                      {ShowModal === "DetailBillByHandling" && (
                        <DetailBillById
                          dataClick={selectIdClick}
                          listBank={listBank}
                          cusType={"KH"}
                        />
                      )}
                    </>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </>
      )}
    </>
  );
};

export default BillPageCustomer;
