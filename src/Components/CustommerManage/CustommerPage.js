import { useMemo, useState, useEffect, useCallback } from "react";
import axios from "axios";
import DataTable from "react-data-table-component";
import CreateCustommer from "./CreateCustommer";

const CustommerPage = () => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);

  const [selectedRows, setSelectedRows] = useState([]);
  const [selectIdClick, setSelectIdClick] = useState([]);

  const columns = useMemo(() => [
    {
      cell: (val) => (
        <button
          className="btn btn-sm btn-warning"
          onClick={() => handleButtonClick(val)}
        >
          Action
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      name: "Title",
      selector: (row) => row.title,
    },
    {
      name: "Year",
      selector: (row) => row.year,
    },
  ]);

  const handleChange = useCallback((state) => {
    setSelectedRows(state.selectedRows);
    console.log(state.selectedRows);
  }, []);

  const handleButtonClick = (val) => {
    // setSelectIdClick(state.selectedRows);

    console.log(val);
  };

  const fetchUsers = async (page) => {
    setLoading(true);

    const response = await axios.get(
      `https://reqres.in/api/users?page=${page}&per_page=${perPage}&delay=1`
    );

    setData(response.data.data);
    setTotalRows(response.data.total);
    setLoading(false);
  };

  const handlePageChange = (page) => {
    fetchUsers(page);
  };

  const handlePerRowsChange = async (newPerPage, page) => {
    setLoading(true);

    const response = await axios.get(
      `https://reqres.in/api/users?page=${page}&per_page=${newPerPage}&delay=1`
    );

    setData(response.data.data);
    setPerPage(newPerPage);
    setLoading(false);
  };

  useEffect(() => {
    setData([
      {
        id: 1,
        title: "Beetlejuice",
        year: "1988",
      },
      {
        id: 2,
        title: "Ghostbusters",
        year: "1984",
      },
    ]);
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
                    className="btn btn-default"
                    data-toggle="modal"
                    data-target="#modal-xl"
                  >
                    <i className="fas fa-plus-circle"></i>
                  </button>
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
          <div className="card-footer">Footer</div>
        </div>
        <div className="modal fade " id="modal-xl" data-backdrop="static">
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
                  aria-label="Close"
                >
                  <span aria-hidden="true">×</span>
                </button>
              </div>
              <div className="modal-body">
                <CreateCustommer />
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
};

export default CustommerPage;
