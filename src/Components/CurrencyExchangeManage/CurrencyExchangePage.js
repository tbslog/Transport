import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import { Modal } from "bootstrap";
import { ToastWarning } from "../Common/FuncToast";
import ConfirmDialog from "../Common/Dialog/ConfirmDialog";
import DatePicker from "react-datepicker";
import { useForm, Controller } from "react-hook-form";
import AddCurrencyExchange from "./AddCurrencyExchange";

const CurrencyExchangePage = () => {
  const {
    register,
    reset,
    setValue,
    control,
    watch,
    validate,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const [data, setData] = useState([]);
  const [page, setPage] = useState(1);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();
  const [fromDate, setFromDate] = useState("");
  const [toDate, setToDate] = useState("");

  const [listCurrencyCode, setListCurrencyCode] = useState([]);
  const [listBanks, setListBanks] = useState([]);

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [listStatus, setListStatus] = useState([]);

  const [isAccept, setIsAccept] = useState();
  const [ShowConfirm, setShowConfirm] = useState(false);

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <>
          <button
            onClick={() =>
              handleChangeValue(val, SetShowModal("Edit"), setTitle("Cập Nhật"))
            }
            type="button"
            className="btn btn-title btn-sm btn-default mx-1"
            gloss="Cập Nhật Thông Tin"
          >
            <i className="far fa-edit"></i>
          </button>
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "ID",
      selector: (row) => row.id,
      omit: true,
    },
    {
      name: "Ngân Hàng",
      selector: (row) => row.bank,
    },
    {
      name: "Mã Loại Tiền Tệ",
      selector: (row) => row.currencyCode,
    },
    {
      name: "Tên Loại Tiền Tệ",
      selector: (row) => row.currencyName,
    },
    {
      name: "Tỉ Giá Bán",
      selector: (row) =>
        !row.priceSell
          ? ""
          : row.priceSell.toLocaleString("vi-VI", {
              style: "currency",
              currency: "VND",
            }),
    },
    {
      name: "Tỉ Giá Mua",
      selector: (row) =>
        !row.priceBuy
          ? ""
          : row.priceBuy.toLocaleString("vi-VI", {
              style: "currency",
              currency: "VND",
            }),
    },
    {
      name: "Tỉ Giá Chuyển Đổi",
      selector: (row) =>
        !row.priceTransfer
          ? ""
          : row.priceTransfer.toLocaleString("vi-VI", {
              style: "currency",
              currency: "VND",
            }),
    },
    {
      name: "Tỉ Giá Tự Chỉnh",
      selector: (row) =>
        !row.priceFix
          ? ""
          : row.priceFix.toLocaleString("vi-VI", {
              style: "currency",
              currency: "VND",
            }),
    },
    {
      name: "Thời Gian Tạo",
      selector: (row) => moment(row.createdTime).format("YYYY-MM-DD HH:mm:ss"),
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

  const handleChangeValue = (val) => {
    setSelectIdClick(val);
    setValue("priceChange", val.priceFix);
    setValue("GhiChu", val.note);
    showModalForm(SetShowModal("UpdatePrice"), setTitle("Ghi Chú"));
  };

  const handlingOnSubmitChangePrice = async () => {
    const id = selectIdClick.id;
    const priceNew = watch("priceChange");
    const note = watch("GhiChu");

    if (!priceNew) {
      ToastWarning("Vui lòng nhập giá điều chỉnh");
      return;
    }
    if (!note) {
      ToastWarning("Vui lòng nhập ghi chú");
      return;
    }

    if (id && priceNew && note) {
      let update = await postData(
        `CurrencyExchange/UpdateExchangeRate?id=${id}&priceFix=${priceNew}&note=${note}`
      );

      if (update === 1) {
        hideModal();
        setValue("priceChange", null);
        setValue("GhiChu", null);
        fetchData(page, keySearch, fromDate, toDate);
      }
    }
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const fetchData = async (page, KeyWord = "", fromDate = "", toDate = "") => {
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    fromDate = fromDate === "" ? "" : moment(fromDate).format("YYYY-MM-DD");
    toDate = toDate === "" ? "" : moment(toDate).format("YYYY-MM-DD");

    const dataCus = await getData(
      `CurrencyExchange/GetListExchangeRate?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&fromDate=${fromDate}&toDate=${toDate}`
    );

    setData(dataCus.data);
    setTotalRows(dataCus.totalRecords);
  };

  const handlePageChange = async (page) => {
    setLoading(true);
    setPage(page);
    await fetchData(page, keySearch, fromDate, toDate);
    setLoading(false);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);
    const dataCus = await getData(
      `CurrencyExchange/GetListExchangeRate?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&fromDate=${fromDate}&toDate=${toDate}`
    );
    setData(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  useEffect(() => {
    setLoading(true);
    (async () => {
      let listCurrencyCode = await getData("Common/LoadCurrencyExchange");
      setListCurrencyCode(listCurrencyCode);

      let listBanks = await getData("Common/GetListBanks");
      setListBanks(listBanks);

      fetchData(1);
      setLoading(false);
    })();
  }, []);

  const handleSearchClick = async () => {
    if (keySearch === "") {
      ToastWarning("Vui lòng nhập thông tin tìm kiếm");
      return;
    }
    await fetchData(1, keySearch, fromDate, toDate);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    await fetchData(1);
  };

  const ShowConfirmDialog = (val) => {
    setSelectIdClick(val);
    setShowConfirm(true);
  };

  const funcAgree = async () => {
    switch (ShowModal) {
      case "CreateAccount":
        const createAcc = await postData(
          `Driver/CreateAccountDriver?driverId=${selectIdClick.maTaiXe}`
        );

        if (createAcc === 1) {
          fetchData(1);
        }
        setShowConfirm(false);
        break;
      default:
        return;
    }
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản Lý Tỉ Giá Ngoại Tệ</h1>
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
                <div className="col-sm-3">
                  <button
                    type="button"
                    className="btn btn-title btn-sm btn-default mx-1"
                    gloss="Tạo Mới Tỉ Giá"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setTitle("Tạo Mới Tỉ Giá")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3"></div>
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
                <div className="col-sm-3 ">
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
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                pagination
                paginationServer
                paginationTotalRows={totalRows}
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
              <div className="col-sm-3"></div>
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
            style={{ maxWidth: "90%" }}
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
                  {ShowModal === "UpdatePrice" && (
                    <div>
                      <div className="row">
                        <div className="col col-sm">
                          <div className="row">
                            <div className="col col-sm">
                              <label>Ngân Hàng</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={selectIdClick.bank}
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Mã Loại Tiền Tệ</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={selectIdClick.currencyCode}
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Tên Loại Tiền Tệ</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={selectIdClick.currencyName}
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Ngày cập nhật</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={moment(selectIdClick.createdTime).format(
                                  "YYYY-MM-DD HH:mm:ss"
                                )}
                              />
                            </div>
                          </div>
                          <div className="row">
                            <div className="col col-sm">
                              <label>Tỉ Giá Bán</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={
                                  !selectIdClick.priceSell
                                    ? ""
                                    : selectIdClick.priceSell
                                }
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Tỉ Giá Mua</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={
                                  !selectIdClick.priceBuy
                                    ? ""
                                    : selectIdClick.priceBuy
                                }
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Tỉ Chuyển Đổi</label>
                              <input
                                disabled={true}
                                className="form-control"
                                autoComplete="false"
                                type="text"
                                value={
                                  !selectIdClick.priceTransfer
                                    ? ""
                                    : selectIdClick.priceTransfer
                                }
                              />
                            </div>
                            <div className="col col-sm">
                              <label>Giá tự chỉnh</label>
                              <input
                                className="form-control"
                                autoComplete="false"
                                type="number"
                                {...register(`priceChange`)}
                              />
                            </div>
                          </div>
                        </div>
                      </div>
                      <div className="row">
                        <div className="col col-sm">
                          <div className="form-group">
                            <label htmlFor="GhiChu">Ghi Chú</label>
                            <textarea
                              autoComplete="false"
                              type="text"
                              className="form-control"
                              id="GhiChu"
                              {...register(`GhiChu`)}
                            ></textarea>
                          </div>
                        </div>
                      </div>
                      <div className="col col-sm">
                        <button
                          onClick={() => handlingOnSubmitChangePrice()}
                          type="submit"
                          className="btn btn-primary"
                          style={{ float: "right" }}
                        >
                          Xác Nhận
                        </button>
                      </div>
                    </div>
                  )}
                  {ShowModal === "Create" && (
                    <AddCurrencyExchange
                      getData={handleRefeshDataClick}
                      listCurrencyCode={listCurrencyCode}
                      listBanks={listBanks}
                      hideModal={hideModal}
                    />
                  )}
                </>
              </div>
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
      </section>
    </>
  );
};

export default CurrencyExchangePage;
