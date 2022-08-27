import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import CreateCustommer from "./CreateCustommer";
import moment from "moment";
import EditCustommer from "./EdtiCustommer";
import { Modal } from "bootstrap";
import { ToastWarning } from "../Common/FuncToast";
import FileExcelImport from "../../ExcelFile/CustommerModule/AddnewCus.xlsx";

const CustommerPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);
  const [keySearch, setKeySearch] = useState("");

  const [ShowModal, SetShowModal] = useState("");
  const [modal, setModal] = useState(null);
  const parseExceptionModal = useRef();

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState({});
  const [Address, SetAddress] = useState({});

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("Edit"))}
          type="button"
          className="btn btn-sm btn-default"
        >
          <i className="far fa-edit"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Mã khách hàng",
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: "Tên khách hàng",
      selector: (row) => row.tenKh,
    },
    {
      name: "Mã số thuế",
      selector: (row) => row.maSoThue,
    },
    {
      name: "Số điện thoại",
      selector: (row) => row.sdt,
    },
    {
      name: "Địa chỉ",
      selector: (row) => row.diaDiem,
    },
    {
      name: "Thời gian cập nhật",
      selector: (row) => row.updateTime,
      sortable: true,
    },
    {
      name: "Thời Gian tạo mới",
      selector: (row) => row.createdtime,
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

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleEditButtonClick = async (val) => {
    showModalForm();
    const dataCus = await getData(
      `http://localhost:8088/api/Custommer/GetCustommerById?CustommerId=${val.maKh}`
    );
    setSelectIdClick(dataCus);
    SetAddress(dataCus.address);
  };

  const fetchData = async (page, KeyWord = "") => {
    setLoading(true);

    if (KeyWord !== "") {
      KeyWord = keySearch;
    }

    const dataCus = await getData(
      `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=${page}&PageSize=${perPage}&KeyWord=${KeyWord}`
    );

    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const dataCus = await getData(
      `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=${page}&PageSize=${newPerPage}&KeyWord=${keySearch}`
    );

    formatTable(dataCus.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  useEffect(() => {
    setLoading(true);

    (async () => {
      let dataCus = await getData(
        `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=1&PageSize=10`
      );

      formatTable(dataCus.data);
      setTotalRows(dataCus.totalRecords);
    })();

    setLoading(false);
  }, []);

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
      "http://localhost:8088/api/Custommer/ReadFileExcel",
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
    await fetchData(1, keySearch);
  };

  const handleRefeshDataClick = async () => {
    setKeySearch("");
    await fetchData(1);
  };

  return (
    <>
      <section className="content-header">
        <div className="container-fluid">
          <div className="row mb-2">
            <div className="col-sm-6">
              <h1>Quản lý thông tin khách hàng</h1>
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
                    className="btn btn-sm btn-default mx-1"
                    onClick={() => showModalForm(SetShowModal("Create"))}
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
                </div>
                <div className="col-sm-3"></div>
                <div className="col-sm-3"></div>
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
                selectableRows
                onSelectedRowsChange={handleChange}
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
              />
            </div>
          </div>
          <div className="card-footer">
            <div className="row">
              <div className="col-sm-3">
                <a
                  href={FileExcelImport}
                  download="Template Thêm mới Khách hàng.xlsx"
                  className="btn btn-sm btn-default mx-1"
                >
                  <i className="fas fa-download"></i>
                </a>
                <div className="upload-btn-wrapper">
                  <button className="btn btn-sm btn-default mx-1">
                    <i className="fas fa-upload"></i>
                  </button>
                  <input
                    type="file"
                    name="myfile"
                    onChange={(e) => handleExcelImportClick(e)}
                  />
                </div>
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
            style={{ maxWidth: "88%" }}
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
                  {ShowModal === "Edit" && (
                    <EditCustommer
                      selectIdClick={selectIdClick}
                      Address={Address}
                      getListUser={fetchData}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <CreateCustommer getListUser={fetchData} />
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
