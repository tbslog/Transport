import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, getDataCustom } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";
import EditCustommer from "./EdtiCustommer";
import CreateCustommer from "./CreateCustommer";
import { Modal } from "bootstrap";
import { ToastWarning } from "../Common/FuncToast";
import FileExcelImport from "../../ExcelFile/CustommerTemplate/AddnewCus.xlsx";

const CustommerPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [page, setPage] = useState(0);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});

  const [listCustomerGroup, setListCustomerGroup] = useState([]);
  const [listCustomerType, setListCustomerType] = useState([]);
  const [listStatus, setListStatus] = useState([]);
  const [ListTypeAddress, SetListTypeAddress] = useState([]);
  const [cusType, setCusType] = useState("KH");

  const [listChuoi, setListChuoi] = useState([]);

  const [title, setTitle] = useState("");

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() =>
            handleEditButtonClick(
              val,
              SetShowModal("Edit"),
              setTitle("Cập Nhật Thông Tin Đối Tác")
            )
          }
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Cập Nhật Thông Tin"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Chuỗi",
      selector: (row) => row.chuoi,
    },
    {
      name: "Mã khách hàng",
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: "Tên khách hàng",
      selector: (row) => row.tenKh,
      sortable: true,
    },
    {
      name: "Phân Loại",
      selector: (row) => row.loaiKH,
      sortable: true,
    },
    {
      name: "Nhóm",
      selector: (row) => row.nhomKH,
      sortable: true,
    },
    {
      name: "Mã số thuế",
      selector: (row) => row.maSoThue,
    },
    {
      name: "Số điện thoại",
      selector: (row) => row.sdt,
    },

    // {
    //   name: "Thời gian cập nhật",
    //   selector: (row) => row.updateTime,
    //   sortable: true,
    // },
    // {
    //   name: "Thời Gian tạo mới",
    //   selector: (row) => row.createdtime,
    //   sortable: true,
    // },
  ]);

  const showModalForm = () => {
    const modal = new Modal(parseExceptionModal.current, {
      keyboard: false,
      backdrop: "static",
    });
    setModal(modal);
    modal.show();
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  useEffect(() => {
    setLoading(true);
    (async () => {
      let getListCustommerGroup = await getData(`Common/GetListCustommerGroup`);
      setListCustomerGroup(getListCustommerGroup);

      let getListChuoi = await getData(`Customer/GetListChuoiSelect`);
      setListChuoi(getListChuoi);

      let getListCustommerType = await getData(`Common/GetListCustommerType`);
      setListCustomerType(getListCustommerType);

      const getListTypeAddress = await getData("address/GetListAddressType");
      if (getListTypeAddress && getListTypeAddress.length > 0) {
        let obj = [];

        getListTypeAddress.map((val) => {
          obj.push({
            value: val.maLoaiDiaDiem,
            label: val.maLoaiDiaDiem + " - " + val.tenLoaiDiaDiem,
          });
        });
        SetListTypeAddress(obj);
      }

      let getStatusList = await getDataCustom(`Common/GetListStatus`, [
        "common",
      ]);
      setListStatus(getStatusList);

      fetchData(1, "", "KH");

      setLoading(false);
    })();
  }, []);

  const handleEditButtonClick = async (val) => {
    showModalForm();
    const dataCus = await getData(`Customer/GetCustomerById?Id=${val.maKh}`);
    setSelectIdClick(dataCus);
  };

  const fetchData = async (page, KeyWord = "", cusType) => {
    setLoading(true);
    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    const dataCus = await getData(
      `Customer/GetListCustomer?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}&customerType=${cusType}`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    setPage(page);
    await fetchData(page, keySearch, cusType);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);
    const dataCus = await getData(
      `Customer/GetListCustomer?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}&customerType=${cusType}`
    );

    formatTable(dataCus.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const ReloadData = async () => {
    await fetchData(page, keySearch, cusType);
  };

  const hideModal = () => {
    modal.hide();
  };

  function formatTable(data) {
    data.map((val) => {
      val.createdtime = moment(val.createdtime).format(" HH:mm:ss DD/MM/YYYY");
      val.updateTime = moment(val.updateTime).format(" HH:mm:ss DD/MM/YYYY");
    });
    setData(data);
  }

  const handleExcelImportClick = async (e) => {
    setLoading(true);
    var file = e.target.files[0];
    e.target.value = null;

    const importExcelCus = await postData(
      "Customer/ReadFileExcel",
      { formFile: file },
      {
        headers: { "Content-Type": "multipart/form-data" },
      }
    );
    setLoading(false);
  };

  const handleSearchClick = async () => {
    if (keySearch === "") {
      ToastWarning("Vui lòng  nhập thông tin tìm kiếm");
      return;
    }

    await fetchData(1, keySearch, cusType);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    setCusType("");
    await fetchData(1, "", "");
  };

  const handleOnChangeCusType = async (val) => {
    setCusType(val);
    await fetchData(1, keySearch, val);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý thông tin đối tác</h1>
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
                    gloss="Thêm Mới Khách Hàng"
                    onClick={() =>
                      showModalForm(
                        SetShowModal("Create"),
                        setTitle("Tạo Mới Thông Tin Đối Tác")
                      )
                    }
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3"></div>
                <div className="col-sm-3">
                  <select
                    className="form-control form-control-sm"
                    onChange={(e) => handleOnChangeCusType(e.target.value)}
                    value={cusType}
                  >
                    <option value={""}>Tất Cả</option>
                    {listCustomerType &&
                      listCustomerType.length > 0 &&
                      listCustomerType.map((val) => {
                        return (
                          <option key={val.maLoaiKh} value={val.maLoaiKh}>
                            {val.tenLoaiKh}
                          </option>
                        );
                      })}
                  </select>
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
                title="Danh sách khách hàng"
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
            style={{ maxWidth: "88%" }}
          >
            <div className="modal-content">
              <div className="modal-header ">
                <h5 className="modal-title">{title}</h5>
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
                  {ShowModal === "Edit" && (
                    <EditCustommer
                      selectIdClick={selectIdClick}
                      getListUser={ReloadData}
                      listCusGroup={listCustomerGroup}
                      listCusType={listCustomerType}
                      listStatus={listStatus}
                      listTypeAddress={ListTypeAddress}
                      listChuoi={listChuoi}
                      hideModal={hideModal}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <CreateCustommer
                      getListUser={ReloadData}
                      listCusGroup={listCustomerGroup}
                      listCusType={listCustomerType}
                      listStatus={listStatus}
                      listTypeAddress={ListTypeAddress}
                      listChuoi={listChuoi}
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

export default CustommerPage;
