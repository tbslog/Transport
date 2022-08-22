import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import axios from "axios";
import DataTable from "react-data-table-component";
import CreateCustommer from "./CreateCustommer";
import moment from "moment";
import EditCustommer from "./EdtiCustommer";
import { Modal } from "bootstrap";

const CustommerPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);

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
      name: "MaKH",
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: "Tên KH",
      selector: (row) => row.tenKh,
    },
    {
      name: "MST",
      selector: (row) => row.maSoThue,
    },
    {
      name: "SĐT",
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
    const modal = new Modal(parseExceptionModal.current, { keyboard: false });
    setModal(modal);
    modal.show();
  };

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
  }, []);

  const handleEditButtonClick = (val) => {
    showModalForm();

    async function getDataCus() {
      const Custommer = await axios.get(
        `http://localhost:8088/api/Custommer/GetCustommerById?CustommerId=${val.maKh}`
      );

      const dataRes = Custommer && Custommer.data ? Custommer.data : {};
      setSelectIdClick(dataRes);
      SetAddress(dataRes.address);
    }
    getDataCus();
  };

  const fetchUsers = async (page) => {
    setLoading(true);

    const response = await axios.get(
      `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=${page}&PageSize=${perPage}`
    );

    console.log(response.data.totalRecords);
    setData(response.data.data);
    setTotalRows(response.data.totalRecords);
    setLoading(false);
  };

  const handlePageChange = async (page) => {
    await fetchUsers(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const response = await axios.get(
      `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=${page}&PageSize=${newPerPage}`
    );

    setData(response.data.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  const hideModal = () => {
    modal.hide();
  };

  useEffect(() => {
    setLoading(true);
    const getData = async () => {
      var res = await axios.get(
        `http://localhost:8088/api/Custommer/GetListCustommer?PageNumber=1&PageSize=10`
      );

      let data = res && res.data ? res.data.data : [];

      data.map((val) => {
        val.createdtime = moment(val.createdtime).format(
          " HH:mm:ss DD/MM/YYYY"
        );
        val.updateTime = moment(val.updateTime).format(" HH:mm:ss DD/MM/YYYY");
      });
      setTotalRows(res.data.totalRecords);
      setData(data);
    };

    getData();
    setLoading(false);
  }, []);

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

                  <button className="btn btn-sm btn-default mx-1" type="button">
                    <i className="fas fa-file-export"></i>
                  </button>
                  <button className="btn btn-sm btn-default mx-1" type="button">
                    <i className="fas fa-upload"></i>
                  </button>
                </div>
                <div className="col-sm-3"></div>
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
          <div className="card-footer">Footer</div>
        </div>
        <div
          className="modal fade "
          id="modal-xl"
          data-backdrop="static"
          ref={parseExceptionModal}
          aria-labelledby="parseExceptionModal"
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
                      getListUser={fetchUsers}
                    />
                  )}
                  {ShowModal === "Create" && (
                    <CreateCustommer getListUser={fetchUsers} />
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
