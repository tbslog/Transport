import { useMemo, useState, useEffect, useCallback, useRef } from "react";
import { getData, postData, getFileImage } from "../Common/FuncAxios";
import { useForm, Controller, useFieldArray } from "react-hook-form";
import DataTable from "react-data-table-component";
import moment from "moment";
import Lightbox from "react-awesome-lightbox";
import "react-awesome-lightbox/build/style.css";
import Cookies from "js-cookie";

const HandlingImage = (props) => {
  const { dataClick, checkModal } = props;
  const accountType = Cookies.get("AccType");

  const {
    register,
    reset,
    setValue,
    control,
    getValues,
    validate,
    formState: { errors },
    handleSubmit,
  } = useForm({
    mode: "onChange",
  });

  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);

  const [ShowModal, SetShowModal] = useState("");

  const [selectIdClick, setSelectIdClick] = useState({});
  const [image, setImage] = useState("");
  const [name, setName] = useState("");

  const handleDeleteImage = async (val) => {
    const fileId = val.id;
    var del = await postData(`BillOfLading/DeleteImage?fileId=${fileId}`);

    if (del === 1) {
      let datafilter = data.filter((x) => x.id !== fileId);
      setData(datafilter);
    }
  };

  const handleOnChangeName = (rowId, newName) => {
    setValue(`ImageName${rowId}`, newName);
  };

  const LoadNameImage = (row) => {
    setValue(`ImageName${row.id}`, row.fileName);
  };

  const handleOnSave = async (rowId) => {
    let newName = getValues(`ImageName${rowId}`);

    if (newName && newName.length > 0) {
      const changeName = await postData(
        `BillOfLading/ChangeImageName?id=${rowId}&newName=${newName}`
      );

      if (changeName === 1) {
      }
    }
  };

  const columns = useMemo(() => [
    {
      //   name: "Xem Hình",
      cell: (val) => (
        <button
          onClick={() => handleEditButtonClick(val, SetShowModal("ShowImage"))}
          type="button"
          className="btn btn-title btn-sm btn-default mx-1"
          gloss="Xem Hình Ảnh"
        >
          <i className="far fa-eye"></i>
        </button>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },
    {
      cell: (val) => (
        <>
          {accountType && accountType === "NV" && (
            <button
              onClick={() => handleDeleteImage(val)}
              type="button"
              className="btn btn-title btn-sm btn-default mx-1"
              gloss="Xóa Hình Ảnh"
            >
              <i className="fas fa-trash"></i>
            </button>
          )}
        </>
      ),
      ignoreRowClick: true,
      allowOverflow: true,
      button: true,
    },

    {
      selector: (row) => row.id,
      omit: true,
    },
    {
      //   name: "Tên Hình",
      selector: (row) => (
        <div className="row">
          <div className="col-sm">
            <input
              autoComplete="false"
              type="text"
              className="form-control"
              id="ImageName"
              {...register(`ImageName${row.id}`)}
              onLoad={LoadNameImage(row)}
              onChange={(e) => handleOnChangeName(row.id, e.target.value)}
            />
          </div>
          <div className="col-sm">
            <button
              type="button"
              onClick={() => handleOnSave(row.id)}
              className="btn-sm mx-1"
            >
              <i className="fas fa-check"></i>
            </button>
          </div>
        </div>
      ),
      sortable: true,
    },
    {
      name: "",
      selector: (row) => row.fileType,
      sortable: true,
    },
    {
      //   name: "Thời Gian Upload",
      selector: (row) => moment(row.uploadedTime).format("DD/MM/YYYY HH:mm:ss"),
      sortable: true,
    },
  ]);

  useEffect(() => {
    if (props && dataClick && Object.keys(dataClick).length > 0) {
      fetchData(dataClick.maDieuPhoi);
    }
  }, [props, dataClick, checkModal]);

  const fetchData = async (maDieuPhoi) => {
    setLoading(true);
    const data = await getData(
      `BillOfLading/GetListImage?handlingId=${maDieuPhoi}`
    );

    setData(data);
    setLoading(false);
  };

  const handleEditButtonClick = (value) => {
    setSelectIdClick(value);
    (async () => {
      var getImage = await getFileImage(
        `BillOfLading/GetImageById?id=${value.id}`
      );
      setName(value.fileName);
      setImage(getImage);
    })();
  };

  return (
    <>
      <section className="content">
        <div className="card">
          <div className="card-header">
            <div className="container-fruid">
              <div className="row">
                {/* <button
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Xác Nhận Đã Đủ Chứng Từ"
                  type="button"
                >
                  <i className="far fa-check-circle"></i>
                </button>
                <button
                  className="btn btn-title btn-sm btn-default mx-1"
                  gloss="Khóa Chứng Từ"
                  type="button"
                >
                  <i className="fas fa-lock"></i>
                </button> */}
              </div>
            </div>
          </div>
          <div className="card-body">
            <div className="container-datatable" style={{ height: "50vm" }}>
              <DataTable
                columns={columns}
                data={data}
                progressPending={loading}
                highlightOnHover
                fixedHeader
                fixedHeaderScrollHeight="60vh"
              />
            </div>
          </div>
          <div className="card-footer"></div>
        </div>

        {ShowModal === "ShowImage" && (
          <div>
            <Lightbox
              images={[{ url: image, title: name }, {}]}
              onClose={() => SetShowModal("HideImage")}
            ></Lightbox>
          </div>
        )}
      </section>
    </>
  );
};

export default HandlingImage;
