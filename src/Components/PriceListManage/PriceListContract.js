import { useState, useEffect, useMemo } from "react";
import { getData } from "../Common/FuncAxios";
import DataTable from "react-data-table-component";
import moment from "moment";

const PriceListContract = (props) => {
  const { selectIdClick, onlyCT, title } = props;

  const columns = useMemo(() => [
    {
      omit: true,
      selector: (row) => row.id,
    },
    {
      name: "Mã Khách Hàng",
      selector: (row) => row.maKh,
      sortable: true,
    },
    {
      name: "Tên Khách Hàng",
      selector: (row) => row.tenKH,
      sortable: true,
    },
    {
      name: "Mã Hợp Đồng",
      selector: (row) => row.maHopDong,
      sortable: true,
    },
    {
      name: "",
      selector: (row) => row.soHopDongCha,
      sortable: true,
    },
    {
      name: "Mã Cung Đường",
      selector: (row) => row.maCungDuong,
      sortable: true,
    },
    {
      name: "Đơn Giá",
      selector: (row) => row.donGia,
      sortable: true,
    },
    {
      name: "Loại Phương Tiện",
      selector: (row) => row.maLoaiPhuongTien,
      sortable: true,
    },
    {
      name: "Loại Hàng Hóa",
      selector: (row) => row.maLoaiHangHoa,
      sortable: true,
    },
    {
      name: "Đơn Vị Tính",
      selector: (row) => row.maDVT,
      sortable: true,
    },
    // {
    //   name: "Phương Thức Vận Chuyển",
    //   selector: (row) => row.maPTVC,
    //   sortable: true,
    // },
    {
      name: "Ngày Áp Dụng",
      selector: (row) => row.ngayApDung,
      sortable: true,
    },
    {
      name: "Ngày Hết Hiệu Lực",
      selector: (row) => row.ngayHetHieuLuc,
      sortable: true,
    },
  ]);

  const [IsLoading, SetIsLoading] = useState(false);
  const [selectedId, setSelectedId] = useState({});
  const [data, setData] = useState([]);
  const [totalRows, setTotalRows] = useState(0);
  const [perPage, setPerPage] = useState(10);

  useEffect(() => {
    if (
      props &&
      selectIdClick &&
      title &&
      Object.keys(selectIdClick).length > 0
    ) {
      setSelectedId(selectIdClick);
    }
  }, [props, selectIdClick, title]);

  useEffect(() => {
    if (selectedId && Object.keys(selectedId).length > 0) {
      fetchData(1);
    }
  }, [selectedId]);

  const handlePerRowsChange = async (newPerPage, page) => {
    SetIsLoading(true);
    const dataCus = await getData(
      `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${newPerPage}&onlyct=${onlyCT}`
    );

    formatTable(dataCus.data);
    setPerPage(newPerPage);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };
  const fetchData = async (page) => {
    SetIsLoading(true);
    const dataCus = await getData(
      `PriceTable/GetListPriceTableByContractId?Id=${selectedId.maHopDong}&PageNumber=${page}&PageSize=${perPage}&onlyct=${onlyCT}`
    );
    formatTable(dataCus.data);
    setTotalRows(dataCus.totalRecords);
    SetIsLoading(false);
  };

  function formatTable(data) {
    data.map((val) => {
      val.ngayApDung = moment(val.ngayApDung).format("DD/MM/YYYY");
      val.ngayHetHieuLuc = moment(val.ngayHetHieuLuc).format("DD/MM/YYYY");
    });
    setData(data);
  }

  const handlePageChange = async (page) => {
    await fetchData(page);
  };

  return (
    <div>
      <div className="card card-primary">
        <div className="card-header">
          <h3 className="card-title">{title}</h3>
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
                onChangeRowsPerPage={handlePerRowsChange}
                onChangePage={handlePageChange}
                highlightOnHover
                striped
                direction="auto"
                responsive
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PriceListContract;
